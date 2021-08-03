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
using System.Threading.Tasks;

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
        internal static InternalManager internalManager;

        SQL sql;
        CfxManager cfx;
        internal static PluginLoader loader;

        public Main()
        {
            Task.Factory.StartNew(async () => 
            {
                await Delay(0);

                logger = new Logger();
                logger.Clear();

                Watermark();

                framework = new Framework();

                sql = new SQL(logger, new SQLConnection("localhost", 3306, "rdr_newcore", "root", ""));
                sql.Connect();

                commandManager = new CommandManager(logger);
                threadManager = new ThreadManager(c => Tick += c, c => Tick -= c);
                eventManager = new EventManager(EventHandlers, logger);
                rpc = new RpcRequest(new RpcHandler(EventHandlers), new RpcTrigger(Players), new RpcSerializer());
                exportManager = new ExportManager(logger);
                user = new UserManager(framework);
                permission = new PermissionManager(framework);
                characterManager = new CharacterManager(framework);
                requestInternalManager = new RequestInternalManager(framework);
                requestManager = new RequestManager(framework);
                syncManager = new SyncManager(framework);
                cfx = new CfxManager(EventHandlers, logger, eventManager);
                internalManager = new InternalManager(logger);

                framework.SetDependencies(threadManager, eventManager, exportManager, syncManager, logger, commandManager, Players, rpc, sql, user, permission, characterManager, requestManager, requestInternalManager, internalManager);

                loader = new PluginLoader(framework);
                loader.Load();

                await loader.IsPluginsFullyLoaded();

                var plugins = loader.Plugins;
                internalManager.SetPlugins(ref plugins);

                framework.IsReadyToWork = true;
            });
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
