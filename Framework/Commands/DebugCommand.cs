using Average.Server.Framework.Attributes;
using Average.Server.Framework.Interfaces;

namespace Average.Server.Framework.Commands
{
    internal class DebugCommand : ICommand
    {
        public DebugCommand()
        {

        }

        [ClientCommand("debug.gotow")]
        private void OnGotow()
        {

        }
    }
}
