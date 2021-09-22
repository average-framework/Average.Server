using System;

namespace Average.Server.Framework.Events
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
