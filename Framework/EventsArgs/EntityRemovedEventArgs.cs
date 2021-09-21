using System;

namespace Average.Server.Framework.EventsArgs
{
    public class EntityRemovedEventArgs : EventArgs
    {
        public int Handle { get; }

        public EntityRemovedEventArgs(int handle)
        {
            Handle = handle;
        }
    }
}
