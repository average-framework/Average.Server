using Average.Internal;
using Average.Plugins;
using Average.Threading;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using SDK.Server;
using SDK.Server.Commands;
using SDK.Server.Diagnostics;
using SDK.Server.Events;
using SDK.Server.Exports;
using SDK.Server.Rpc;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Average
{
    internal class Main : BaseScript
    {
        static EventHandlerDictionary eventHandlers;
        static PlayerList players;

        internal static Logger logger;
        internal static CommandManager commandManager;
        internal static Framework framework;
        internal static ThreadManager threadManager;
        internal static EventManager eventManager;
        internal static ExportManager exportManager;
        internal static SDK.Server.SyncManager syncManager;
        internal static RpcRequest rpc;

        SQL sql;
        SyncManager sync;
        CfxManager cfx;
        PluginLoader loader;

        public Main()
        {
            eventHandlers = EventHandlers;
            players = Players;

            logger = new Logger();
            logger.Clear();
            Watermark();
            sql = new SQL(logger, new SQLConnection("localhost", 3306, "rdr_newcore", "root", ""));

            commandManager = new CommandManager(logger);
            threadManager = new ThreadManager(this);
            eventManager = new EventManager(EventHandlers, logger);
            rpc = new RpcRequest(new SDK.Shared.Rpc.RpcHandler(EventHandlers), new RpcTrigger(Players), new SDK.Shared.Rpc.RpcSerializer());
            exportManager = new ExportManager(logger);
            syncManager = new SDK.Server.SyncManager(logger);
            framework = new Framework(threadManager, eventManager, exportManager, syncManager, logger, commandManager, Players, rpc);
            cfx = new CfxManager(EventHandlers, logger, framework);
            loader = new PluginLoader(rpc, logger, commandManager);

            loader.Load();

            sync = new SyncManager(syncManager);
            RegisterScript(sync);
        }

        internal void RegisterTick(Func<Task> func)
        {
            Tick += func;
        }

        internal void UnregisterTick(Func<Task> func)
        {
            Tick -= func;
        }

        void Watermark()
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
            Console.WriteLine("");
        }
    }
}
