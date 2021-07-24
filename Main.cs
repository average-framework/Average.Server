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
using SDK.Shared.Rpc;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Average
{
    internal class Main : BaseScript
    {
        internal static EventHandlerDictionary Events { get; private set; }
        internal static ExportDictionary ScriptExports { get; private set; }
        internal static PlayerList PlayerList { get; set; }

        internal static Logger logger;
        internal static CommandManager commandManager;
        internal static Framework framework;
        internal static ThreadManager threadManager;
        internal static EventManager eventManager;
        internal static ExportManager exportManager;

        PluginLoader plugin;

        public Main()
        {
            Events = EventHandlers;
            ScriptExports = Exports;
            PlayerList = Players;

            logger = new Logger();
            commandManager = new CommandManager(logger);
            threadManager = new ThreadManager(this);
            eventManager = new EventManager(EventHandlers, logger);
            framework = new Framework(EventHandlers, ScriptExports, threadManager, eventManager, exportManager, Players, logger, commandManager);
            plugin = new PluginLoader(commandManager);

            logger.Clear();
            Watermark();
            plugin.Load();
        }

        internal static RpcRequest Event(string @event)
        {
            return new RpcRequest(@event, new RpcHandler(Events), new RpcTrigger(PlayerList), new RpcSerializer());
        }

        /// <summary>
        /// Create new thread at runtime
        /// </summary>
        /// <param name="task"></param>
        internal void RegisterTick(Func<Task> func)
        {
            Tick += func;
        }

        /// <summary>
        /// Delete thread at runtime
        /// </summary>
        /// <param name="task"></param>
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
