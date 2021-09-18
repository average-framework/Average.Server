using System;

namespace Average.Server.Framework.Events
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
