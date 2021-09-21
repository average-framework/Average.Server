using System;

namespace Average.Server.Framework.EventsArgs
{
    public class EntityCreatingEventArgs : EventArgs
    {
        public int Handle { get; }

        public EntityCreatingEventArgs(int handle)
        {
            Handle = handle;
        }
    }
}
