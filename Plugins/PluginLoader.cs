using CitizenFX.Core;
using Newtonsoft.Json;
using SDK.Server;
using SDK.Server.Commands;
using SDK.Server.Plugins;
using SDK.Shared;
using SDK.Shared.Plugins;
using SDK.Shared.Rpc;
using SDK.Shared.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static CitizenFX.Core.Native.API;
using static SDK.Server.Rpc.IRpcRequest;

namespace Average.Plugins
{
    internal class PluginLoader
    {
        CommandManager commandManager;

        string BASE_RESOURCE_PATH = GetResourcePath(SDK.Shared.Constant.RESOURCE_NAME);

        List<Plugin> plugins = new List<Plugin>();

        public PluginLoader(CommandManager commandManager)
        {
            this.commandManager = commandManager;

            Main.Event("avg.internal.get_plugins").On(new Action<RpcMessage, RpcCallback>(GetPluginsEvent));
        }

        IEnumerable<string> GetPluginsPath()
        {
            var pluginsDirectoryPath = string.Join("/", BASE_RESOURCE_PATH, SDK.Shared.Constant.BASE_PLUGIN_DIRECTORY_NAME);
            return Directory.GetDirectories(pluginsDirectoryPath);
        }

        IEnumerable<string> ValidatePlugins()
        {
            var pluginsPath = GetPluginsPath();

            if (pluginsPath.Count() == 0)
            {
                Main.logger.Warn($"No plugins detected");
                return null;
            }

            var validate = new List<string>();

            foreach (var dir in pluginsPath)
            {
                var dirInfo = new DirectoryInfo(dir);
                var pluginFiles = GetPluginFiles(dir);

                if (pluginFiles.Count() == 0)
                {
                    Main.logger.Error($"[{dirInfo.Name.ToUpper()}] Is not a valid resource.");
                    continue;
                }

                foreach (var pluginFile in pluginFiles)
                {
                    var fileInfo = new FileInfo(pluginFile);

                    switch (fileInfo.Name)
                    {
                        case Constant.BASE_PLUGIN_MANIFEST_FILENAME:
                            var pluginInfo = GetPluginInfo(fileInfo);

                            if (pluginInfo != null)
                            {
                                var integrity = CheckPluginManifestIntegrity(pluginFile);

                                if (integrity)
                                {
                                    validate.Add(pluginFile);
                                }
                                else
                                {
                                    Main.logger.Error($"[{dirInfo.Name.ToUpper()}] Invalid plugin format.");
                                }
                            }
                            else
                            {
                                Main.logger.Error($"[{dirInfo.Name.ToUpper()}] Invalid plugin info.");
                            }
                            break;
                    }
                }

                Main.logger.Warn($"{validate.Count} validated plugins of {pluginsPath.Count()} detected !");
            }

            return validate;
        }

        bool CheckPluginManifestIntegrity(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            foreach (var key in Configuration.pluginFileFormatKeys)
            {
                if (!obj.ContainsKey(key))
                {
                    return false;
                }
            }

            return true;
        }

        PluginInfo GetPluginInfo(FileInfo fileInfo)
        {
            try
            {
                var content = File.ReadAllText(fileInfo.FullName);
                return JsonConvert.DeserializeObject<PluginInfo>(content);
            }
            catch
            {
                throw new FormatException($"[ERROR][{fileInfo.Name.ToUpper()}] {Constant.BASE_PLUGIN_MANIFEST_FILENAME} have an invalid format.");
            }
        }

        IEnumerable<string> GetPluginFiles(string filePath)
        {
            return Directory.GetFiles(filePath);
        }

        public void Load()
        {
            //foreach (var dir in Directory.GetDirectories(Path.Combine(GetResourcePath(GetCurrentResourceName()), "plugins")))
            //{
            //    foreach(var include in Directory.GetFiles(dir, "*.net.dll"))
            //    {
            //        var fileInfo = new FileInfo(include);

            //        AppDomain.CurrentDomain.Load(File.ReadAllBytes(include));
            //    }
            //}

            foreach (var file in ValidatePlugins())
            {
                var currentDirPath = Path.GetDirectoryName(file);
                var currentDirName = Utilities.Directory.GetDirectoryName(currentDirPath);

                var fileInfo = new FileInfo(file);
                var pluginInfo = GetPluginInfo(fileInfo);

                if (pluginInfo != null)
                {
                    if (!string.IsNullOrEmpty(pluginInfo.Server))
                    {
                        var serverFile = Path.Combine(currentDirPath, pluginInfo.Server);
                        var asm = Assembly.LoadFrom(serverFile);
                        var sdk = asm.GetCustomAttribute<ServerPluginAttribute>();

                        if (sdk != null)
                        {
                            var asmName = asm.GetName().ToString().Split(',')[0];

                            Main.logger.Info($"Loading {asmName} ...");

                            var types = asm.GetTypes().Where(x => !x.IsAbstract && x.IsClass && x.IsSubclassOf(typeof(Plugin))).ToList();
                            var mainScriptCount = 0;

                            foreach (var type in types)
                            {
                                var attr = type.GetCustomAttribute<MainScriptAttribute>();

                                if (attr != null)
                                {
                                    mainScriptCount++;
                                }
                            }

                            if (mainScriptCount > 1)
                            {
                                Main.logger.Error("Unable to load multiples [MainScript] attribute in same plugin. Fix this error to continue.");
                                return;
                            }

                            if (mainScriptCount == 0)
                            {
                                Main.logger.Error("Unable to load this plugin, he does not contains [MainScript] attribute. Fix this error to continue.");
                                return;
                            }

                            foreach (var type in types)
                            {
                                object classObj = null;

                                if (type.GetCustomAttribute<MainScriptAttribute>() != null)
                                {
                                    try
                                    {
                                        // Activate asm instance
                                        classObj = Activator.CreateInstance(type, Main.framework, pluginInfo);
                                        var plugin = classObj as Plugin;
                                        plugin.PluginInfo = pluginInfo;
                                        RegisterPlugin(plugin);

                                        Main.logger.Info($"{asmName} Successfully loaded.");
                                    }
                                    catch (InvalidCastException ex)
                                    {
                                        Main.logger.Error($"Unable to load {asmName}");
                                    }
                                }

                                if(classObj == null)
                                {
                                    continue;
                                }

                                RegisterCommands(type, classObj);
                                RegisterThreads(type, classObj);
                                RegisterEvents(asm, type, classObj);
                            }
                        }
                        else
                        {
                            Main.logger.Error($"[{currentDirName.ToUpper()}] {fileInfo.Name} is not an valid plugin for AverageFramework.");
                        }
                    }
                    else
                    {
                        Main.logger.Error($"[{currentDirName.ToUpper()}] {Constant.BASE_PLUGIN_MANIFEST_FILENAME} does not contains value for the \"Server\" key. Set the value like this: \"your_plugin.server.net.dll\".");
                    }
                }
            }
        }

        void RegisterCommands(Type type, object classObj)
        {
            // Load registered commands (method need to be public to be detected)
            foreach (var method in type.GetMethods())
            {
                var cmdAttr = method.GetCustomAttribute<SDK.Server.CommandAttribute>();
                var aliasAttr = method.GetCustomAttribute<CommandAliasAttribute>();

                commandManager.RegisterCommand(cmdAttr, aliasAttr, method, classObj);
            }
        }

        void RegisterThreads(Type type, object classObj)
        {
            foreach (var method in type.GetMethods())
            {
                var threadAttr = method.GetCustomAttribute<ThreadAttribute>();

                if (threadAttr != null)
                {
                    Main.threadManager.RegisterThread(method, threadAttr, classObj);
                }
            }
        }

        void RegisterEvents(Assembly asm, Type type, object classObj)
        {
            foreach (var method in type.GetMethods())
            {
                var eventAttr = method.GetCustomAttribute<EventAttribute>();

                if (eventAttr != null)
                {
                    Main.logger.Debug("Params: " + string.Join(", ", method.GetParameters().Select(x => x.ParameterType.FullName)));
                    //var paramType = Type.GetType()
                    //var action = method.Invoke(classObj, new object[] { "test1", "action2"});
                    Main.eventManager.RegisterEvent(method, eventAttr, classObj);
                    //Main.eventManager.RegisterEvent(method, eventAttr, classObj);
                }
            }
        }

        void RegisterPlugin(Plugin script)
        {
            BaseScript.RegisterScript(script);
            plugins.Add(script);
        }

        void UnloadScript(Plugin script)
        {
            BaseScript.UnregisterScript(script);
            plugins.Remove(script);
        }

        #region Events

        void GetPluginsEvent(RpcMessage message, RpcCallback callback)
        {
            var pluginsInfo = new List<IPluginInfo>();

            Main.logger.Warn("Get plugins for client");

            foreach (var plugin in plugins)
            {
                pluginsInfo.Add(plugin.PluginInfo);
            }

            callback(pluginsInfo);
        }

        #endregion
    }
}
