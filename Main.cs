using Average.Server.Data;
using Average.Server.Managers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Server.Rpc;
using SDK.Shared.Rpc;
using System;
using System.Reflection;

namespace Average.Server
{
    internal class Main : BaseScript
    {
        internal static EventHandlerDictionary eventHandlers;
        internal static PlayerList players;

        internal static CommandManager commandManager;
        internal static Framework framework;
        internal static ThreadManager threadManager;
        internal static EventManager eventManager;
        internal static ExportManager exportManager;
        internal static SyncManager syncManager;
        internal static RpcRequest rpc;
        internal static UserManager user;
        internal static PermissionManager permission;
        internal static CharacterManager characterManager;
        internal static RequestManager requestManager;
        internal static RequestInternalManager requestInternalManager;
        internal static JobManager jobManager;
        internal static DoorManager doorManager;
        internal static SaveManager saveManager;
        
        internal static PluginLoader loader;

        internal static SQL sql;

        internal static Main instance;
        
        CfxManager cfxManager;

        public Main()
        {
            instance = this;
            eventHandlers = EventHandlers;
            players = Players;
            
            Log.Clear();

            Watermark();

            var baseConfig = SDK.Server.Configuration.Parse("config.json");
            sql = new SQL(new SQLConnection((string)baseConfig["MySQL"]["Host"], (int)baseConfig["MySQL"]["Port"], (string)baseConfig["MySQL"]["Database"], (string)baseConfig["MySQL"]["Username"], (string)baseConfig["MySQL"]["Password"]));
            sql.Connect();

            // Internal Script
            commandManager = new CommandManager();
            threadManager = new ThreadManager(c => Tick += c, c => Tick -= c);
            eventManager = new EventManager();
            syncManager = new SyncManager();
            exportManager = new ExportManager();
            rpc = new RpcRequest(new RpcHandler(EventHandlers), new RpcTrigger(Players), new RpcSerializer());
            requestInternalManager = new RequestInternalManager();
            requestManager = new RequestManager();

            saveManager = new SaveManager();

            user = new UserManager();
            permission = new PermissionManager();
            characterManager = new CharacterManager();
            jobManager = new JobManager();
            doorManager = new DoorManager();
            cfxManager = new CfxManager();

            // Framework Script
            framework = new Framework(threadManager, eventManager, exportManager, syncManager, commandManager, Players, rpc, sql, user, permission, characterManager, requestManager, requestInternalManager, jobManager, doorManager, saveManager);

            // Plugin Loader
            loader = new PluginLoader();
            loader.Load();
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
