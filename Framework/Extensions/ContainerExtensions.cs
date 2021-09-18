using Average.Server.Framework.Extensions;
using DryIoc;
using System;
using System.Collections.Generic;

namespace Average.Server.Framework.Extensions
{
    public static class ContainerExtensions
    {
        private static readonly List<Type> _instances = new List<Type>();

        public static void Register<T>(this IContainer container)
        {
            container.Register<T>(Reuse.Singleton);
            container.Resolve(typeof(T));
        }

        public static T GetService<T>(this IContainer container)
        {
            return (T)container.GetService(typeof(T));
        }
    }
}
