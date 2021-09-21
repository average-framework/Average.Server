using System;

namespace Average.Server.Framework.EventsArgs
{
    public class ServerResourceStopEventArgs : EventArgs
    {
        public string Resource { get; }

        public ServerResourceStopEventArgs(string resource)
        {
            Resource = resource;
        }
    }
}
