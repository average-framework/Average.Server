using System;

namespace Average.Server.Framework.EventsArgs
{
    public class ServerResourceStartEventArgs : EventArgs
    {
        public string Resource { get; }

        public ServerResourceStartEventArgs(string resource)
        {
            Resource = resource;
        }
    }
}
