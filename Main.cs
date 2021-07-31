using Average.Data;
using Average.Managers;
using Average.Plugins;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Server.Rpc;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Average
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
        internal static InternalManager internalManager;

        SQL sql;
        CfxManager cfx;
        PluginLoader loader;

        public Main()
        {
            logger = new Logger();
            //logger.Clear();
            //Watermark();

            sql = new SQL(logger, new SQLConnection("localhost", 3306, "rdr_newcore", "root", ""));
            sql.Connect();
            commandManager = new CommandManager(logger);
            threadManager = new ThreadManager(this);
            eventManager = new EventManager(EventHandlers, logger);
            rpc = new RpcRequest(new SDK.Shared.Rpc.RpcHandler(EventHandlers), new RpcTrigger(Players), new SDK.Shared.Rpc.RpcSerializer());
            exportManager = new ExportManager(logger);
            syncManager = new SyncManager(logger);
            internalManager = new InternalManager(logger);
            framework = new Framework(threadManager, eventManager, exportManager, syncManager, logger, commandManager, Players, rpc, sql, internalManager);
            cfx = new CfxManager(EventHandlers, logger, eventManager);
            loader = new PluginLoader(rpc, logger, commandManager);
            internalManager.SetPluginList(ref loader.plugins);

            //Task.Factory.StartNew(async () => 
            //{
            //    while (!sql.IsOpen)
            //    {
            //        logger.Warn("Trying to reconnect to database in 5 seconds");
            //        await Delay(5000);
            //        sql.Connect();
            //    }

            //    loader.Load();
            //    Tick += syncManager.SyncUpdate;
            //});

            loader.Load();
            Tick += syncManager.SyncUpdate;
        }

        internal void RegisterTick(Func<Task> func)
        {
            Tick += func;
        }

        internal void UnregisterTick(Func<Task> func)
        {
            Tick -= func;
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
            Console.WriteLine("");
        }
    }
}
