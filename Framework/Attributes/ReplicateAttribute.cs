using System;

namespace Average.Server.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReplicateAttribute : Attribute
    {
        public string Name { get; }

        public ReplicateAttribute(string name)
        {
            Name = name;
        }
    }
}
