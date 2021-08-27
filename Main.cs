using Average.Server.Data;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using SDK.Server.Diagnostics;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Average.Server
{
    internal class Main : BaseScript
    {
        internal static Main instance;
        internal static PluginLoader loader;

        internal static Action<Func<Task>> attachCallback;
        internal static Action<Func<Task>> detachCallback;

        internal static EventHandlerDictionary eventHandlers;
        
        public Main()
        {
            instance = this;
            eventHandlers = EventHandlers;
            
            attachCallback = c => Tick += c;
            detachCallback = c => Tick -= c;
            
            Log.Clear();
            Watermark();

            // Internal Script
            SQL.Init();
            SQL.Connect();

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
