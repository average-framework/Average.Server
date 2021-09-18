using System;

namespace Average.Server.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ExportAttribute : Attribute
    {
        public string Name { get; set; }

        public ExportAttribute(string name)
        {
            Name = name;
        }
    }
}
