using System;

namespace Average.Server.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class GetSyncAttribute : Attribute
    {
        public string Name { get; set; }

        public GetSyncAttribute(string name)
        {
            Name = name;
        }
    }
}
