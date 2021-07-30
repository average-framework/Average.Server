using Average.Managers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Server.Plugins;
using SDK.Server.Rpc;
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
using static SDK.Server.Rpc.RpcRequest;

namespace Average.Plugins
{
    public class PluginLoader : BaseScript
    {
        RpcRequest rpc;
        Logger logger;
        CommandManager commandManager;

        string BASE_RESOURCE_PATH = GetResourcePath(Constant.RESOURCE_NAME);

        public List<IPlugin> plugins = new List<IPlugin>();
        List<PluginInfo> clientPlugins = new List<PluginInfo>();

        public PluginLoader(RpcRequest rpc, Logger logger, CommandManager commandManager)
        {
            this.rpc = rpc;
            this.logger = logger;
            this.commandManager = commandManager;

            rpc.Event("avg.internal.get_plugins").On(new Action<RpcMessage, RpcCallback>(GetPluginsEvent));
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
                logger.Warn($"No plugins detected.");
                return null;
            }

            var validate = new List<string>();

            foreach (var dir in pluginsPath)
            {
                var dirInfo = new DirectoryInfo(dir);
                var pluginFiles = GetPluginFiles(dir);

                if (pluginFiles.Count() == 0)
                {
                    logger.Error($"[{dirInfo.Name.ToUpper()}] Is not a valid resource.");
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
                                    logger.Error($"[{dirInfo.Name.ToUpper()}] Invalid plugin format.");
                                }
                            }
                            else
                            {
                                logger.Error($"[{dirInfo.Name.ToUpper()}] Invalid plugin info.");
                            }
                            break;
                    }
                }
            }

            logger.Warn($"{validate.Count} validated plugins of {pluginsPath.Count()} detected !");
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
                    if (!string.IsNullOrEmpty(pluginInfo.Client))
                    {
                        // Is client only
                        clientPlugins.Add(pluginInfo);
                    }
                    else
                    {
                        //logger.Error($"[{currentDirName.ToUpper()}] {Constant.BASE_PLUGIN_MANIFEST_FILENAME} does not contains value for the \"Server\" key. Set the value like this: \"your_plugin.server.net.dll\".");
                    }

                    if (!string.IsNullOrEmpty(pluginInfo.Server))
                    {
                        var serverFile = Path.Combine(currentDirPath, pluginInfo.Server);
                        var asm = Assembly.LoadFrom(serverFile);
                        var sdk = asm.GetCustomAttribute<ServerPluginAttribute>();

                        if (sdk != null)
                        {
                            var asmName = asm.GetName().ToString().Split(',')[0];

                            logger.Info($"Loading {asmName} ...");

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
                                logger.Error("Unable to load multiples [MainScript] attribute in same plugin. Fix this error to continue.");
                                return;
                            }

                            if (mainScriptCount == 0)
                            {
                                logger.Error("Unable to load this plugin, he does not contains [MainScript] attribute. Fix this error to continue.");
                                return;
                            }

                            foreach (var type in types)
                            {
                                Plugin script = null;

                                if (type.GetCustomAttribute<MainScriptAttribute>() != null)
                                {
                                    try
                                    {
                                        // Activate asm instance
                                        script = (Plugin)Activator.CreateInstance(type, Main.framework, pluginInfo);
                                        //script.PluginInfo = pluginInfo;
                                        RegisterPlugin(script);

                                        logger.Info($"Plugin {asm.GetName().Name} -> script: {script.Name} successfully loaded.");
                                    }
                                    catch (InvalidCastException ex)
                                    {
                                        logger.Error($"Unable to load {asm.GetName().Name}");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        script = (Plugin)Activator.CreateInstance(type, Main.framework, pluginInfo);
                                        //script.PluginInfo = pluginInfo;
                                        RegisterPlugin(script);

                                        logger.Info($"Plugin {asm.GetName().Name} -> script: {script.Name} successfully loaded.");
                                    }
                                    catch
                                    {
                                        logger.Error($"Unable to load script: {script.Name}");
                                    }
                                }

                                if (script == null)
                                {
                                    continue;
                                }

                                RegisterThreads(type, script);
                                RegisterEvents(type, script);
                                RegisterExports(type, script);
                                RegisterSyncs(type, script);
                                RegisterGetSyncs(type, script);
                                RegisterCommands(type, script);
                            }
                        }
                        else
                        {
                            logger.Error($"[{currentDirName.ToUpper()}] {fileInfo.Name} is not an valid plugin for AverageFramework.");
                        }
                    }
                    else
                    {
                        //logger.Error($"[{currentDirName.ToUpper()}] {Constant.BASE_PLUGIN_MANIFEST_FILENAME} does not contains value for the \"Server\" key. Set the value like this: \"your_plugin.server.net.dll\".");
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

        void RegisterEvents(Type type, object classObj)
        {
            foreach (var method in type.GetMethods())
            {
                var eventAttr = method.GetCustomAttribute<EventAttribute>();

                if (eventAttr != null)
                {
                    Main.eventManager.RegisterEvent(method, eventAttr, classObj);
                }
            }
        }

        void RegisterExports(Type type, object classObj)
        {
            foreach (var method in type.GetMethods())
            {
                var exportAttr = method.GetCustomAttribute<ExportAttribute>();

                if (exportAttr != null)
                {
                    Main.exportManager.RegisterExport(method, exportAttr, classObj);
                }
            }
        }

        void RegisterSyncs(Type type, object classObj)
        {
            // Registering syncs (method need to be public to be detected)
            for (int i = 0; i < type.GetProperties().Count(); i++)
            {
                var property = type.GetProperties()[i];
                var syncAttr = property.GetCustomAttribute<SyncAttribute>();

                if (syncAttr != null)
                {
                    Main.syncManager.RegisterSync(ref property, syncAttr, classObj);
                }
            }

            for (int i = 0; i < type.GetFields().Count(); i++)
            {
                var field = type.GetFields()[i];
                var syncAttr = field.GetCustomAttribute<SyncAttribute>();

                if (syncAttr != null)
                {
                    Main.syncManager.RegisterSync(ref field, syncAttr, classObj);
                }
            }

            // Registering networkSyncs (property need to be public to be detected)
            for (int i = 0; i < type.GetProperties().Count(); i++)
            {
                var property = type.GetProperties()[i];
                var syncAttr = property.GetCustomAttribute<NetworkSyncAttribute>();

                if (syncAttr != null)
                {
                    Main.syncManager.RegisterNetworkSync(ref property, syncAttr, classObj);
                }
            }

            // Registering networkSyncs (field need to be public to be detected)
            for (int i = 0; i < type.GetFields().Count(); i++)
            {
                var field = type.GetFields()[i];
                var syncAttr = field.GetCustomAttribute<NetworkSyncAttribute>();

                if (syncAttr != null)
                {
                    Main.syncManager.RegisterNetworkSync(ref field, syncAttr, classObj);
                }
            }
        }

        void RegisterGetSyncs(Type type, object classObj)
        {
            // Registering getSyncs (property need to be public to be detected)
            for (int i = 0; i < type.GetProperties().Count(); i++)
            {
                var property = type.GetProperties()[i];
                var getSyncAttr = property.GetCustomAttribute<GetSyncAttribute>();

                if (getSyncAttr != null)
                {
                    Main.syncManager.RegisterGetSync(ref property, getSyncAttr, classObj);
                }
            }

            // Registering getSyncs (field need to be public to be detected)
            for (int i = 0; i < type.GetFields().Count(); i++)
            {
                var field = type.GetFields()[i];
                var getSyncAttr = field.GetCustomAttribute<GetSyncAttribute>();

                if (getSyncAttr != null)
                {
                    Main.syncManager.RegisterGetSync(ref field, getSyncAttr, classObj);
                }
            }
        }

        void RegisterPlugin(Plugin script)
        {
            //BaseScript.RegisterScript(script);
            plugins.Add(script);
        }

        void UnloadScript(Plugin script)
        {
            //BaseScript.UnregisterScript(script);
            plugins.Remove(script);
        }

        #region Events

        void GetPluginsEvent(RpcMessage message, RpcCallback callback)
        {
            var pluginsInfo = new List<IPluginInfo>();

            foreach (var plugin in clientPlugins)
            {
                pluginsInfo.Add(plugin);
            }

            callback(pluginsInfo);
        }

        #endregion
    }
}
