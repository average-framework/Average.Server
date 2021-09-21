using System;

namespace Average.Server.Framework.EventsArgs
{
    public class PlayerEnteredScopeEventArgs : EventArgs
    {
        public object Data { get; }

        public PlayerEnteredScopeEventArgs(object data)
        {
            Data = data;
        }
    }
}
