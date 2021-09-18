using MemBus;
using System;

namespace Average.Server.Framework.Extensions
{
    public interface IEvent
    {

    }

    public static class MembusExtensions
    {
        public static void SubscribeOnScriptingThread<T>(this IBus bus, Action<T> subscription) where T : IEvent => bus.Subscribe<T>(obj => subscription.Invoke(obj));
    }
}
