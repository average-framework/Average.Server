using System;

namespace Average.Server.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServerCommandAttribute : Attribute
    {
        public string Command { get; set; }

        public ServerCommandAttribute(string command)
        {
            Command = command;
        }
    }
}
