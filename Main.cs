using Average.Server.Data;
using Average.Server.Managers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using DryIoc;
using Newtonsoft.Json.Linq;
using SDK.Server.Diagnostics;
using SDK.Server.Rpc;
using SDK.Shared.Rpc;
using System;
using System.Threading.Tasks;

namespace Average.Server
{
    internal class Main : BaseScript
    {
        internal static SQL sql;
        internal static RpcRequest rpc;

        internal static Action<Func<Task>> attachCallback;
        internal static Action<Func<Task>> detachCallback;

        //internal static PluginLoader loader;

        #region Internal Scripts

        //internal static readonly EventManager evnt = new EventManager();
        //internal static readonly ExportManager export = new ExportManager();
        //internal static readonly PermissionManager permission = new PermissionManager();
        //internal static readonly RequestManager request = new RequestManager();
        //internal static readonly RequestInternalManager requestInternal = new RequestInternalManager();
        //internal static readonly SaveManager save = new SaveManager();
        //internal static readonly SyncManager sync = new SyncManager();
        //internal static readonly ThreadManager thread = new ThreadManager();
        //internal static readonly JobManager job = new JobManager();
        //internal static readonly DoorManager door = new DoorManager();
        //internal static readonly CfxManager cfx = new CfxManager();
        //internal static readonly StorageManager storage = new StorageManager();
        //internal static readonly EnterpriseManager enterprise = new EnterpriseManager();

        #endregion

        public readonly bool isDebugEnabled;

        private readonly JObject _baseConfig;

        private readonly IContainer _container;
        private readonly Bootstrapper _boostrap;

        public Main()
        {
            Logger.Clear();
            Watermark();

            _container = new Container().With(rules => rules.WithFactorySelector(Rules.SelectLastRegisteredFactory()));
            _boostrap = new Bootstrapper(_container, EventHandlers, Players);

            _baseConfig = SDK.Server.Configuration.ParseToObj("config.json");

            isDebugEnabled = (bool)_baseConfig["IsDebugModeEnabled"];

            Logger.IsDebug = isDebugEnabled;

            rpc = new RpcRequest(new RpcHandler(EventHandlers), new RpcTrigger(Players), new RpcSerializer());

            attachCallback = c => Tick += c;
            detachCallback = c => Tick -= c;

            // rpc = new RpcRequest(new RpcHandler(EventHandlers), new RpcTrigger(Players), new RpcSerializer());

            //sql = new SQL();
            //sql.Connect();

            //loader = new PluginLoader();

            //LoadInternalScript(request);
            //LoadInternalScript(permission);
            //LoadInternalScript(evnt);
            //LoadInternalScript(export);
            //LoadInternalScript(thread);
            //LoadInternalScript(command);
            //LoadInternalScript(sync);
            //LoadInternalScript(save);
            //LoadInternalScript(user);
            //LoadInternalScript(job);
            //LoadInternalScript(character);
            //LoadInternalScript(storage);
            //LoadInternalScript(door);
            //LoadInternalScript(cfx);
            //LoadInternalScript(enterprise);

            // Plugin Loader
            //loader.Load();
        }

        #region Console Command

        //[Command("clear")]
        //private void ClearCommand() => Console.Clear();

        #endregion

        //internal void LoadInternalScript(InternalPlugin script)
        //{
        //    try
        //    {
        //        script.SetDependencies(sql, Players, new RpcRequest(new RpcHandler(eventHandlers), new RpcTrigger(Players), new RpcSerializer()), thread, character, command, evnt, export, permission, save, sync, user, request, requestInternal, job, door, storage, enterprise);

        //        loader.RegisterThreads(script.GetType(), script);
        //        loader.RegisterEvents(script.GetType(), script);
        //        loader.RegisterExports(script.GetType(), script);
        //        loader.RegisterSyncs(script.GetType(), script);
        //        loader.RegisterGetSyncs(script.GetType(), script);
        //        loader.RegisterCommands(script.GetType(), script);
        //        loader.RegisterInternalPlugin(script);

        //        script.OnInitialized();

        //        Log.Write("Internal", $"% {script.Name} % registered successfully.", new Log.TextColor(ConsoleColor.Blue, ConsoleColor.White));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error($"Unable to loading script: {script.Name}. Error: {ex.Message}\n{ex.StackTrace}.");
        //    }
        //}

        internal void Watermark()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("             AAAAAAAA VVVVV       VVVVV GGGGGGGGGGGGGG");
            Console.WriteLine("            AAAAAAAAA VVVVV      VVVVV GGGGGGGGGGGGGGGGGGG");
            Console.WriteLine("           AAAAAAAAAA VVVVV     VVVVV GGGGGGG      GGGGGGGG");
            Console.WriteLine("          AAAAA AAAAA VVVVV    VVVVV GGGGGGG         GGGGGGG");
            Console.WriteLine("         AAAAA  AAAAA VVVVV   VVVVV GGGGGGGGGGG      GGGGGGG");
            Console.WriteLine("        AAAAA   AAAAA VVVVV  VVVVV GGGGGGGGGGGGG     GGGGGGG");
            Console.WriteLine("       AAAAA    AAAAA VVVVV VVVVV                  GGGGGGGGG");
            Console.WriteLine("      AAAAA     AAAAA VVVVVVVVVV       GGGGGGGGGGGGGGGGGGGG");
            Console.WriteLine("     AAAAA      AAAAA VVVVVVVVV       GGGGGGGGGGGGGGGGGGGG");
            Console.WriteLine("    AAAAA       AAAAA VVVVVVVV       GGGGGGGGGGGGGGGGG");
            Console.WriteLine("    --------------------------------------------------------");
            Console.WriteLine($"    |                 DEV BUILD | {API.GetConvar("sv_maxclients", "")} SLOTS                 |");
            Console.WriteLine("    --------------------------------------------------------");
            Console.WriteLine("");
        }
    }
}
