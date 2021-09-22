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
        internal readonly Action<Func<Task>> _attachCallback;
        internal readonly Action<Func<Task>> _detachCallback;

        private readonly IContainer _container;
        private readonly Bootstrapper _boostrap;

        public Main()
        {
            Logger.Clear();
            Watermark();

            _attachCallback = c => Tick += c;
            _detachCallback = c => Tick -= c;

            _container = new Container().With(rules => rules.WithFactorySelector(Rules.SelectLastRegisteredFactory()));
            _boostrap = new Bootstrapper(this, _container, EventHandlers, Players);
        }

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
