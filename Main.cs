using Average.Server.Data;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using SDK.Server.Diagnostics;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Average.Server.Managers;
using SDK.Server.Rpc;
using SDK.Shared.Rpc;

namespace Average.Server
{
    internal class Main : BaseScript
    {
        internal static SQL sql;
        internal static RpcRequest rpc;

        internal static Action<Func<Task>> attachCallback;
        internal static Action<Func<Task>> detachCallback;

        internal static EventHandlerDictionary eventHandlers;
        internal static PluginLoader loader;

        #region Internal Scripts

        internal static readonly CharacterManager character = new CharacterManager();
        internal static readonly CommandManager command = new CommandManager();
        internal static readonly EventManager evnt = new EventManager();
        internal static readonly ExportManager export = new ExportManager();
        internal static readonly PermissionManager permission = new PermissionManager();
        internal static readonly RequestManager request = new RequestManager();
        internal static readonly RequestInternalManager requestInternal = new RequestInternalManager();
        internal static readonly SaveManager save = new SaveManager();
        internal static readonly SyncManager sync = new SyncManager();
        internal static readonly ThreadManager thread = new ThreadManager();
        internal static readonly UserManager user = new UserManager();
        internal static readonly JobManager job = new JobManager();
        internal static readonly DoorManager door = new DoorManager();

        #endregion
        
        public Main()
        {
            eventHandlers = EventHandlers;
            
            attachCallback = c => Tick += c;
            detachCallback = c => Tick -= c;
            
            Log.Clear();
            Watermark();

            rpc = new RpcRequest(new RpcHandler(EventHandlers), new RpcTrigger(Players), new RpcSerializer());
            
            sql = new SQL();
            sql.Connect();

            loader = new PluginLoader();
            // loader.Preload();

            LoadInternalScript(request);
            LoadInternalScript(permission);
            LoadInternalScript(evnt);
            LoadInternalScript(export);
            LoadInternalScript(thread);
            LoadInternalScript(command);
            LoadInternalScript(sync);
            LoadInternalScript(save);
            LoadInternalScript(user);
            LoadInternalScript(job);
            LoadInternalScript(character);
            LoadInternalScript(door);

            // Plugin Loader
            loader.Load();
        }

        #region Console Command

        [Command("clear")]
        private void ClearCommand()
        {
            Console.Clear();
        }

        #endregion
        
        internal void LoadInternalScript(InternalPlugin script)
        {
            try
            {
                script.SetDependencies(sql, Players, rpc, thread, character, command, evnt, export, permission, save, sync, user, request, requestInternal, job, door);
                
                loader.RegisterThreads(script.GetType(), script);
                loader.RegisterEvents(script.GetType(), script);
                loader.RegisterExports(script.GetType(), script);
                loader.RegisterSyncs(script.GetType(), script);
                loader.RegisterGetSyncs(script.GetType(), script);
                loader.RegisterCommands(script.GetType(), script);
                loader.RegisterInternalPlugin(script);
                
                script.OnInitialized();
                
                Log.Write("Internal", $"% {script.Name} % registered successfully.", new Log.TextColor(ConsoleColor.Blue, ConsoleColor.White));
            }
            catch (Exception ex)
            {
                Log.Error($"Unable to loading script: {script.Name}. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }

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
            Console.WriteLine($"    | VERSION {Assembly.GetExecutingAssembly().GetName().Version} | EARLY BUILD | {API.GetConvar("sv_maxclients", "")} SLOTS      |");
            Console.WriteLine("    --------------------------------------------------------");
            Console.WriteLine("");
        }
    }
}
