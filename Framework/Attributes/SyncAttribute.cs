using System;

namespace Average.Server.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SyncAttribute : Attribute
    {
        public string Name { get; }

        public SyncAttribute(string name)
        {
            Name = name;
        }
    }
}
