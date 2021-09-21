using System;

namespace Average.Server.Framework.EventsArgs
{
    public class ResourceStopEventArgs : EventArgs
    {
        public string Resource { get; }

        public ResourceStopEventArgs(string resource)
        {
            Resource = resource;
        }
    }
}
