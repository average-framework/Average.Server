using DryIoc;
using System;
using System.Collections.Generic;

namespace Average.Server.Extensions
{
    public static class ContainerExtensions
    {
        private static readonly List<Type> _instances = new List<Type>();

        //public static void BindSingletonAndInstanciateOnStartup<T>(this IContainer container) where T : class
        //{
        //    container.Register<T>(Reuse.Singleton);
            
        //    _instances.Add(typeof(T));
        //}

        //public static void InstanciateDefinedBindings(this IContainer container) => _instances.ForEach(t => container.Resolve(t));

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
