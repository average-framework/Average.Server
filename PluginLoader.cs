using CitizenFX.Core;
using Newtonsoft.Json;
using SDK.Server;
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
using Average.Server.Managers;
using SDK.Server.Diagnostics;
using SDK.Server.Rpc;
using static CitizenFX.Core.Native.API;
using static SDK.Server.Rpc.RpcRequest;

namespace Average.Server
{
    public class PluginLoader : BaseScript
    {
        private bool _isReady;
        private string BASE_RESOURCE_PATH = GetResourcePath(Constant.RESOURCE_NAME);

        private readonly List<PluginInfo> _clientPlugins = new List<PluginInfo>();
        private readonly List<InternalPlugin> _internalPlugins = new List<InternalPlugin>();
        private readonly List<Plugin> _plugins = new List<Plugin>();

        private const BindingFlags ReflectionFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance |
            BindingFlags.FlattenHierarchy;
        
        public PluginLoader()
        {
            #region Rpc
        
            var rpc = new RpcRequest(new RpcHandler(EventHandlers), new RpcTrigger(new PlayerList()), new RpcSerializer());
            rpc.Event("avg.internal.get_plugins").On(GetPluginsEvent);
        
            #endregion
        }

        public async Task IsReady()
        {
            while (!_isReady) await Delay(0);
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
                return new string[] { };
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
                            var integrity = CheckPluginManifestIntegrity(pluginFile);

                            if (pluginInfo == null)
                            {
                                Log.Error($"[{dirInfo.Name.ToUpper()}] Invalid plugin info.");
                                continue;
                            }

                            if (integrity)
                            {
                                validate.Add(pluginFile);
                            }
                            else
                            {
                                Log.Error($"[{dirInfo.Name.ToUpper()}] Invalid plugin format.");
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
                    return false;
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
                throw new FormatException(
                    $"[ERROR][{fileInfo.Name.ToUpper()}] {Constant.BASE_PLUGIN_MANIFEST_FILENAME} have an invalid format.");
            }
        }

        private IEnumerable<string> GetPluginFiles(string filePath)
        {
            return Directory.GetFiles(filePath);
        }

        public void Preload()
        {
            foreach (var dir in Directory.GetDirectories(Path.Combine(GetResourcePath(GetCurrentResourceName()), "plugins")))
            {
                foreach(var include in Directory.GetFiles(dir, "*.net.dll"))
                {
                    // var fileInfo = new FileInfo(include);

                    AppDomain.CurrentDomain.Load(File.ReadAllBytes(include));
                    Log.Warn("Preload: " + include);
                }
            }
        }
        
        public void Load()
        {
            // LoadInternalScripts();

            // foreach (var file in ValidatePlugins())
            // {
            //     var currentDirPath = Path.GetDirectoryName(file);
            //
            //     var fileInfo = new FileInfo(file);
            //     var pluginInfo = GetPluginInfo(fileInfo);
            //
            //     if (pluginInfo is null) continue;
            //
            //     // If this plugin is a client plugin
            //     if (!string.IsNullOrEmpty(pluginInfo.Client))
            //         _clientPlugins.Add(pluginInfo);
            //
            //     if (!string.IsNullOrEmpty(pluginInfo.Server))
            //     {
            //         try
            //         {
            //             var serverFile = Path.Combine(currentDirPath, pluginInfo.Server);
            //             var asm = Assembly.LoadFrom(serverFile);
            //             var asmName = asm.GetName().ToString().Split(',')[0];
            //
            //             Log.Info($"Loading {asmName} ...");
            //
            //             var plugins = asm.GetTypes().Where(x => !x.IsAbstract && x.IsClass && x.IsSubclassOf(typeof(Plugin))).ToList();
            //
            //             foreach (var type in plugins)
            //             {
            //                 try
            //                 {
            //                     var script = (Plugin) Activator.CreateInstance(type);
            //
            //                     RegisterThreads(type, script);
            //                     RegisterEvents(type, script);
            //                     RegisterExports(type, script);
            //                     RegisterSyncs(type, script);
            //                     RegisterGetSyncs(type, script);
            //                     RegisterCommands(type, script);
            //                     RegisterPlugin(script);
            //
            //                     Log.Info($"Script: {script.Name} registered successfully.");
            //                 }
            //                 catch (Exception ex)
            //                 {
            //                     Log.Error($"Unable to registering script: {type}. Error: {ex.Message}\n{ex.StackTrace}");
            //                 }
            //             }
            //             
            //             foreach (var script in _plugins)
            //             {
            //                 try
            //                 {
            //                     // SetScriptComponents(script, pluginInfo);
            //                     script.LoadConfiguration();
            //                     script.OnInitialized();
            //                     
            //                     Log.Info($"Script: {script.GetType()} OnInitialized called successfully.");
            //                 }
            //                 catch (Exception ex)
            //                 {
            //                     Log.Error($"Unable to call OnInitialized. Error: {ex.Message}\n{ex.StackTrace}");
            //                 }
            //             }
            //         }
            //         catch
            //         {
            //             Log.Error($"Unable to load this plugin: {pluginInfo.Server} because this plugin does not exist or is an invalid Average Framework plugin.");
            //         }
            //     }
            // }
            
            _isReady = true;
        }

        internal T GetInternalInstance<T>()
        {
            var result = _internalPlugins.Find(x => x.GetType() == typeof(T));
            return (T) Convert.ChangeType(result, typeof(T));
        }

        internal void RegisterCommands(Type type, object classObj)
        {
            // Load registered commands
            foreach (var method in type.GetMethods(ReflectionFlags))
            {
                var cmdAttr = method.GetCustomAttribute<ServerCommandAttribute>();
                
                if (cmdAttr != null)
                    CommandManager.RegisterInternalCommand(cmdAttr, classObj, method);
            }
        }

        internal void RegisterThreads(Type type, object classObj)
        {
            foreach (var method in type.GetMethods(ReflectionFlags))
            {
                var threadAttr = method.GetCustomAttribute<ThreadAttribute>();

                if (threadAttr != null)
                    ThreadManager.RegisterInternalThread(method, threadAttr, classObj);
            }
        }

        internal void RegisterEvents(Type type, object classObj)
        {
            foreach (var method in type.GetMethods(ReflectionFlags))
            {
                var eventAttr = method.GetCustomAttribute<ServerEventAttribute>();

                if (eventAttr != null)
                    EventManager.RegisterInternalEvent(method, eventAttr, classObj);
            }
        }

        internal void RegisterExports(Type type, object classObj)
        {
            foreach (var method in type.GetMethods(ReflectionFlags))
            {
                var exportAttr = method.GetCustomAttribute<ExportAttribute>();

                if (exportAttr != null)
                    ExportManager.RegisterInternalExport(method, exportAttr, classObj);
            }
        }

        internal void RegisterSyncs(Type type, object classObj)
        {
            var properties = type.GetProperties(ReflectionFlags);
            var fields = type.GetFields(ReflectionFlags);

            // Registering syncs
            for (int i = 0; i < properties.Count(); i++)
            {
                var property = properties[i];
                var syncAttr = property.GetCustomAttribute<SyncAttribute>();

                if (syncAttr != null)
                    SyncManager.RegisterInternalSync(ref property, syncAttr, classObj);
            }

            for (int i = 0; i < fields.Count(); i++)
            {
                var field = fields[i];
                var syncAttr = field.GetCustomAttribute<SyncAttribute>();

                if (syncAttr != null)
                    SyncManager.RegisterInternalSync(ref field, syncAttr, classObj);
            }

            // Registering networkSyncs
            for (int i = 0; i < properties.Count(); i++)
            {
                var property = properties[i];
                var syncAttr = property.GetCustomAttribute<NetworkSyncAttribute>();

                if (syncAttr != null)
                    SyncManager.RegisterInternalNetworkSync(ref property, syncAttr, classObj);
            }

            // Registering networkSyncs
            for (int i = 0; i < fields.Count(); i++)
            {
                var field = fields[i];
                var syncAttr = field.GetCustomAttribute<NetworkSyncAttribute>();

                if (syncAttr != null)
                    SyncManager.RegisterInternalNetworkSync(ref field, syncAttr, classObj);
            }
        }

        internal void RegisterGetSyncs(Type type, object classObj)
        {
            var properties = type.GetProperties(ReflectionFlags);
            var fields = type.GetFields(ReflectionFlags);

            // Registering getSyncs
            for (int i = 0; i < properties.Count(); i++)
            {
                var property = properties[i];
                var getSyncAttr = property.GetCustomAttribute<GetSyncAttribute>();

                if (getSyncAttr != null)
                    SyncManager.RegisterInternalGetSync(ref property, getSyncAttr, classObj);
            }

            // Registering getSyncs
            for (int i = 0; i < fields.Count(); i++)
            {
                var field = fields[i];
                var getSyncAttr = field.GetCustomAttribute<GetSyncAttribute>();

                if (getSyncAttr != null)
                    SyncManager.RegisterInternalGetSync(ref field, getSyncAttr, classObj);
            }
        }

        internal void RegisterInternalPlugin(InternalPlugin script)
        {
            _internalPlugins.Add(script);
        }

        internal void UnloadInternalScript(InternalPlugin script)
        {
            _internalPlugins.Remove(script);
        }

        private void RegisterPlugin(Plugin script)
        {
            _plugins.Add(script);
        }

        private void UnloadScript(Plugin script)
        {
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