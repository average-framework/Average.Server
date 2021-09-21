using System;

namespace Average.Server.Framework.EventsArgs
{
    public class EntityCreatedEventArgs : EventArgs
    {
        public int Handle { get; }

        public EntityCreatedEventArgs(int handle)
        {
            Handle = handle;
        }
    }
}
