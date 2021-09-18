using System;

namespace Average.Server.Framework.Events
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
