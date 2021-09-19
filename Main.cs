using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Rpc;
using Average.Server.Framework.Utilities;
using Average.Shared.Rpc;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using DryIoc;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Average.Server
{
    internal class Main : BaseScript
    {
        internal readonly RpcRequest _rpc;

        internal readonly Action<Func<Task>> attachCallback;
        internal readonly Action<Func<Task>> detachCallback;

        private readonly IContainer _container;
        private readonly Bootstrapper _boostrap;

        public Main()
        {
            Logger.Clear();
            Watermark();

            attachCallback = c => Tick += c;
            detachCallback = c => Tick -= c;

            _rpc = new RpcRequest(new RpcHandler(EventHandlers), new RpcTrigger(Players), new RpcSerializer());

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
