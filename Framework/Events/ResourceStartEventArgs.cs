using System;

namespace Average.Server.Framework.Events
{
    public class ResourceStartEventArgs : EventArgs
    {
        public string Resource { get; }

        public ResourceStartEventArgs(string resource)
        {
            Resource = resource;
        }
    }
}
