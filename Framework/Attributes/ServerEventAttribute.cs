using System;

namespace Average.Server.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServerEventAttribute : Attribute
    {
        public string Event { get; }

        public ServerEventAttribute(string @event)
        {
            Event = @event;
        }
    }
}
