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
        internal static Logger logger;
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

        internal static PluginLoader loader;

        public Main()
        {
            logger = new Logger();
            logger.Clear();

            Watermark();

            var baseConfig = SDK.Server.Configuration.Parse("config.json");
            var sql = new SQL(logger, new SQLConnection((string)baseConfig["MySQL"]["Host"], (int)baseConfig["MySQL"]["Port"], (string)baseConfig["MySQL"]["Database"], (string)baseConfig["MySQL"]["Username"], (string)baseConfig["MySQL"]["Password"]));
            sql.Connect();

            // Internal Script
            commandManager = new CommandManager(logger);
            threadManager = new ThreadManager(c => Tick += c, c => Tick -= c);
            eventManager = new EventManager(EventHandlers, logger);
            rpc = new RpcRequest(new RpcHandler(EventHandlers), new RpcTrigger(Players), new RpcSerializer());
            exportManager = new ExportManager(logger);
            user = new UserManager(logger, rpc, sql, Players);
            permission = new PermissionManager(logger, rpc, sql, EventHandlers, Players);
            characterManager = new CharacterManager(logger, rpc, sql, eventManager, Players, EventHandlers);
            requestInternalManager = new RequestInternalManager(logger, eventManager);
            requestManager = new RequestManager(requestInternalManager);
            syncManager = new SyncManager(logger, threadManager);
            var cfx = new CfxManager(EventHandlers, logger, eventManager);
            jobManager = new JobManager(logger, characterManager, EventHandlers, Players);
            doorManager = new DoorManager(logger, EventHandlers, eventManager, rpc);

            // Framework Script
            framework = new Framework(threadManager, eventManager, exportManager, syncManager, logger, commandManager, Players, rpc, sql, user, permission, characterManager, requestManager, requestInternalManager, jobManager, doorManager);

            // Plugin Loader
            loader = new PluginLoader(logger, commandManager, rpc);
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
