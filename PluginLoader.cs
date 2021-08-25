using Average.Server.Managers;
using CitizenFX.Core;
using Newtonsoft.Json;
using SDK.Server;
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
using System.Threading.Tasks;
using SDK.Server.Diagnostics;
using static CitizenFX.Core.Native.API;
using static SDK.Server.Rpc.RpcRequest;

namespace Average.Server
{
    public class PluginLoader : BaseScript
    {
        private bool isReady;
        private string BASE_RESOURCE_PATH = GetResourcePath(Constant.RESOURCE_NAME);

        private readonly List<PluginInfo> _clientPlugins = new List<PluginInfo>();
        private readonly List<Plugin> _plugins = new List<Plugin>();


        public PluginLoader()
        {
            Main.rpc.Event("avg.internal.get_plugins").On(GetPluginsEvent);
        }

        public async Task IsReady()
        {
            while (!isReady) await BaseScript.Delay(0);
        }

        private IEnumerable<string> GetPluginsPath()
        {
            var pluginsDirectoryPath = string.Join("/", BASE_RESOURCE_PATH, SDK.Shared.Constant.BASE_PLUGIN_DIRECTORY_NAME);
            return Directory.GetDirectories(pluginsDirectoryPath);
        }

        private IEnumerable<string> ValidatePlugins()
        {
            var pluginsPath = GetPluginsPath();

            if (pluginsPath.Count() == 0)
            {
                Log.Warn($"No plugins detected.");
                return null;
            }

            var validate = new List<string>();

            foreach (var dir in pluginsPath)
            {
                var dirInfo = new DirectoryInfo(dir);
                var pluginFiles = GetPluginFiles(dir);

                if (pluginFiles.Count() == 0)
                {
                    Log.Error($"[{dirInfo.Name.ToUpper()}] Is not a valid resource.");
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
                                    Log.Error($"[{dirInfo.Name.ToUpper()}] Invalid plugin format.");
                                }
                            }
                            else
                            {
                                Log.Error($"[{dirInfo.Name.ToUpper()}] Invalid plugin info.");
                            }
                            break;
                    }
                }
            }

            Log.Warn($"{validate.Count} validated plugins of {pluginsPath.Count()} detected !");
            return validate;
        }

        private bool CheckPluginManifestIntegrity(string filePath)
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

        private PluginInfo GetPluginInfo(FileInfo fileInfo)
        {
            try
            {
                var content = File.ReadAllText(fileInfo.FullName);
                var pluginInfo = JsonConvert.DeserializeObject<PluginInfo>(content);
                pluginInfo.Name = fileInfo.Directory.Name;
                return pluginInfo;
            }
            catch
            {
                throw new FormatException($"[ERROR][{fileInfo.Name.ToUpper()}] {Constant.BASE_PLUGIN_MANIFEST_FILENAME} have an invalid format.");
            }
        }

        private IEnumerable<string> GetPluginFiles(string filePath)
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

                var fileInfo = new FileInfo(file);
                var pluginInfo = GetPluginInfo(fileInfo);

                if (pluginInfo != null)
                {
                    if (!string.IsNullOrEmpty(pluginInfo.Client))
                    {
                        // Is client only
                        _clientPlugins.Add(pluginInfo);
                    }

                    if (!string.IsNullOrEmpty(pluginInfo.Server))
                    {
                        try
                        {
                            var serverFile = Path.Combine(currentDirPath, pluginInfo.Server);
                            var asm = Assembly.LoadFrom(serverFile);
                            var asmName = asm.GetName().ToString().Split(',')[0];

                            Log.Info($"Loading {asmName} ...");

                            var types = asm.GetTypes().Where(x => !x.IsAbstract && x.IsClass && x.IsSubclassOf(typeof(Plugin))).ToList();
                            var mainScriptCount = types.Select(type => type.GetCustomAttribute<MainScriptAttribute>()).Count(attr => attr != null);

                            if (mainScriptCount > 1)
                            {
                                Log.Error("Unable to load multiples [MainScript] attribute in same plugin. Fix this error to continue.");
                                return;
                            }

                            if (mainScriptCount == 0)
                            {
                                Log.Error("Unable to load this plugin, he does not contains [MainScript] attribute. Fix this error to continue.");
                                return;
                            }

                            foreach (var type in types)
                            {
                                Plugin? script = null;

                                if (type.GetCustomAttribute<MainScriptAttribute>() != null)
                                {
                                    try
                                    {
                                        // Activate asm instance
                                        script = (Plugin)Activator.CreateInstance(type, Main.framework, pluginInfo);
                                        RegisterPlugin(script, type);

                                        Log.Info($"Plugin {asm.GetName().Name} -> script: {script.Name} successfully loaded.");
                                    }
                                    catch (InvalidCastException ex)
                                    {
                                        Log.Error($"Unable to load {asm.GetName().Name}");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        script = (Plugin)Activator.CreateInstance(type, Main.framework, pluginInfo);
                                        RegisterPlugin(script, type);

                                        Log.Info($"Plugin {asm.GetName().Name} -> script: {script.Name} successfully loaded.");
                                    }
                                    catch
                                    {
                                        Log.Error($"Unable to load script: {script.Name}");
                                    }
                                }

                                if (script == null) continue;

                                RegisterThreads(type, script);
                                RegisterEvents(type, script);
                                RegisterExports(type, script);
                                RegisterSyncs(type, script);
                                RegisterGetSyncs(type, script);
                                RegisterCommands(type, script);
                            }
                        }
                        catch
                        {
                            Log.Error($"Unable to load this plugin: {pluginInfo.Server} because this plugin does not exist or is an invalid Average Framework plugin.");
                        }
                    }
                    else
                    {
                        //Log.Error($"[{currentDirName.ToUpper()}] {Constant.BASE_PLUGIN_MANIFEST_FILENAME} does not contains value for the \"Server\" key. Set the value like this: \"your_plugin.server.net.dll\".");
                    }
                }
            }

            isReady = true;
        }

        private void RegisterCommands(Type type, object classObj)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            // Load registered commands
            foreach (var method in type.GetMethods(flags))
            {
                var cmdAttr = method.GetCustomAttribute<ServerCommandAttribute>();
                Main.commandManager.RegisterCommand(cmdAttr, method, classObj);
            }
        }

        private void RegisterThreads(Type type, object classObj)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            foreach (var method in type.GetMethods(flags))
            {
                var threadAttr = method.GetCustomAttribute<ThreadAttribute>();

                if (threadAttr != null)
                {
                    Main.threadManager.RegisterThread(method, threadAttr, classObj);
                }
            }
        }

        private void RegisterEvents(Type type, object classObj)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            foreach (var method in type.GetMethods(flags))
            {
                var eventAttr = method.GetCustomAttribute<ServerEventAttribute>();

                if (eventAttr != null)
                {
                    Main.eventManager.RegisterEvent(method, eventAttr, classObj);
                }
            }
        }

        private void RegisterExports(Type type, object classObj)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            foreach (var method in type.GetMethods(flags))
            {
                var exportAttr = method.GetCustomAttribute<ExportAttribute>();

                if (exportAttr != null)
                {
                    Main.exportManager.RegisterExport(method, exportAttr, classObj);
                }
            }
        }

        private void RegisterSyncs(Type type, object classObj)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            // Registering syncs
            for (int i = 0; i < type.GetProperties(flags).Count(); i++)
            {
                var property = type.GetProperties(flags)[i];
                var syncAttr = property.GetCustomAttribute<SyncAttribute>();

                if (syncAttr != null)
                {
                    Main.syncManager.RegisterSync(ref property, syncAttr, classObj);
                }
            }

            for (int i = 0; i < type.GetFields(flags).Count(); i++)
            {
                var field = type.GetFields(flags)[i];
                var syncAttr = field.GetCustomAttribute<SyncAttribute>();

                if (syncAttr != null)
                {
                    Main.syncManager.RegisterSync(ref field, syncAttr, classObj);
                }
            }

            // Registering networkSyncs
            for (int i = 0; i < type.GetProperties(flags).Count(); i++)
            {
                var property = type.GetProperties(flags)[i];
                var syncAttr = property.GetCustomAttribute<NetworkSyncAttribute>();

                if (syncAttr != null)
                {
                    Main.syncManager.RegisterNetworkSync(ref property, syncAttr, classObj);
                }
            }

            // Registering networkSyncs
            for (int i = 0; i < type.GetFields(flags).Count(); i++)
            {
                var field = type.GetFields(flags)[i];
                var syncAttr = field.GetCustomAttribute<NetworkSyncAttribute>();

                if (syncAttr != null)
                {
                    Main.syncManager.RegisterNetworkSync(ref field, syncAttr, classObj);
                }
            }
        }

        private void RegisterGetSyncs(Type type, object classObj)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            // Registering getSyncs
            for (int i = 0; i < type.GetProperties(flags).Count(); i++)
            {
                var property = type.GetProperties(flags)[i];
                var getSyncAttr = property.GetCustomAttribute<GetSyncAttribute>();

                if (getSyncAttr != null)
                {
                    Main.syncManager.RegisterGetSync(ref property, getSyncAttr, classObj);
                }
            }

            // Registering getSyncs
            for (int i = 0; i < type.GetFields(flags).Count(); i++)
            {
                var field = type.GetFields(flags)[i];
                var getSyncAttr = field.GetCustomAttribute<GetSyncAttribute>();

                if (getSyncAttr != null)
                {
                    Main.syncManager.RegisterGetSync(ref field, getSyncAttr, classObj);
                }
            }
        }

        private void RegisterPlugin(Plugin script, Type classType)
        {
            //BaseScript.RegisterScript(script);
            _plugins.Add(script);
        }

        private void UnloadScript(Plugin script)
        {
            //BaseScript.UnregisterScript(script);
            _plugins.Remove(script);
        }

        #region Event

        private void GetPluginsEvent(RpcMessage message, RpcCallback callback)
        {
            var pluginsInfo = new List<IPluginInfo>();

            foreach (var plugin in _clientPlugins)
                pluginsInfo.Add(plugin);

            callback(pluginsInfo);
        }

        #endregion
    }
}
