using System;

namespace Average.Server.Framework.EventsArgs
{
    public class PlayerLeftScopeEventArgs : EventArgs
    {
        public object Data { get; }

        public PlayerLeftScopeEventArgs(object data)
        {
            Data = data;
        }
    }
}
