using Average.Server.Framework.Diagnostics;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using DryIoc;
using System;
using System.Threading.Tasks;

namespace Average.Server
{
    internal class Main : BaseScript
    {
        private readonly Action<Func<Task>> _addTick;
        private readonly Action<Func<Task>> _removeTick;

        private static Main _instance;

        public Main()
        {
            _instance = this;

            Logger.Clear();
            Watermark();

            _addTick = task => Tick += task;
            _removeTick = task => Tick -= task;

            var container = new Container().With(rules => rules.WithFactorySelector(Rules.SelectLastRegisteredFactory()));
            var boostrap = new Bootstrapper(container, EventHandlers, Players);
        }

        internal static void AddTick(Func<Task> func) => _instance._addTick(func);
        internal static void RemoveTick(Func<Task> func) => _instance._removeTick(func);

        #region Console Command

        [Command("clear")]
        private void ClearCommand() => Console.Clear();

        #endregion

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
