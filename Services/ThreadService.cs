using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    public class ThreadService : IService
    {
        public class Thread
        {
            public bool isStartDelayTriggered;

            public int StartDelay { get; }
            public int RepeatedCount { get; set; }
            public Func<Task> Func { get; set; }
            public bool IsRunning { get; set; } = true;
            public bool IsTerminated { get; set; } = false;
            public MethodInfo Method { get; }

            public Thread(MethodInfo method, int startDelay)
            {
                Method = method;
                StartDelay = startDelay;
            }
        }
        
        private readonly IContainer _container;
        private readonly List<Thread> _threads = new List<Thread>();
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        public ThreadService(IContainer container)
        {
            _container = container;

            Logger.Write("ThreadService", "Initialized successfully");
        }

        internal void Reflect()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();

            foreach (var type in types)
            {
                if (_container.IsRegistered(type))
                {
                    // Continue if the service have the same type of this class
                    if (type == GetType()) continue;

                    // Get service instance
                    var service = _container.GetService(type);
                    var methods = type.GetMethods(flags);

                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<ThreadAttribute>();
                        if (attr == null) continue;

                        RegisterInternalThread(attr, service, method);
                    }
                }
            }
        }

        internal void RegisterInternalThread(ThreadAttribute threadAttr, object classObj, MethodInfo method)
        {
            var methodParams = method.GetParameters();

            if (methodParams.Length == 0)
            {
                if (threadAttr != null)
                {
                    var thread = new Thread(method, threadAttr.StartDelay);
                    Func<Task> func = null;

                    func = async () =>
                    {
                        if (thread.StartDelay > -1)
                        {
                            if (!thread.isStartDelayTriggered)
                            {
                                thread.isStartDelayTriggered = true;
                            }
                        }

                        await (Task)method.Invoke(classObj, new object[] { });

                        var currentThreadIndex = _threads.FindIndex(x => x.Func == func);

                        if (currentThreadIndex != -1)
                        {
                            var currentThread = _threads[currentThreadIndex];

                            if (threadAttr.RepeatCount > 0)
                            {
                                currentThread.RepeatedCount++;

                                if (currentThread.RepeatedCount >= threadAttr.RepeatCount)
                                {
                                    _threads[_threads.FindIndex(x => x.Func == func)].IsRunning = false;
                                    _threads[_threads.FindIndex(x => x.Func == func)].IsTerminated = true;

                                    Main.RemoveTick(func);
                                }
                            }
                        }
                    };

                    thread.Func = func;

                    _threads.Add(thread);
                    Main.AddTick(func);

                    Logger.Debug($"Registering [Thread]: {classObj.GetType()} to method: {method.Name}.");
                }
            }
            else
            {
                Logger.Error($"Unable to register [Thread] to method: {method.Name}, you need to delete parameters: [{string.Join(", ", methodParams.Select(x => x.ParameterType.Name))}]");
            }
        }

        public void StartThread(Func<Task> action) => Main.AddTick(action);
        public void StopThread(Func<Task> action) => Main.RemoveTick(action);

        public IEnumerable<Thread> GetThreads() => _threads.AsEnumerable();
    }
}
