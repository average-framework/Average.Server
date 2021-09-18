using System;

namespace Average.Server.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NetworkSyncAttribute : Attribute
    {
        public string Name { get; }

        public NetworkSyncAttribute(string name)
        {
            Name = name;
        }
    }
}
