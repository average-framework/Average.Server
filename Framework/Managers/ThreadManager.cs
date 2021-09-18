using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Average.Server.Framework.Managers
{
    public class ThreadManager
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

        private readonly List<Thread> _threads = new List<Thread>();

        private readonly IContainer _container;
        private Action<Func<Task>> _attachCallback;
        private Action<Func<Task>> _detachCallback;

        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        public ThreadManager(IContainer container, Action<Func<Task>> attachCallback, Action<Func<Task>> detachCallback)
        {
            _container = container;
            _attachCallback = attachCallback;
            _detachCallback = detachCallback;

            Logger.Write("ThreadManager", "Initialized successfully");
        }

        internal void Reflect()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();

            foreach (var service in types)
            {
                if (_container.IsRegistered(service))
                {
                    // Continue if the service have the same type of this class
                    if (service == GetType()) continue;

                    // Get service instance
                    var _service = _container.GetService(service);
                    var methods = service.GetMethods(flags);

                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<ThreadAttribute>();
                        if (attr == null) continue;

                        RegisterInternalThread(attr, _service, method);
                    }
                }
            }
        }

        internal void RegisterInternalThread(ThreadAttribute threadAttr, object classObj, MethodInfo method)
        {
            var methodParams = method.GetParameters();

            if (methodParams.Count() == 0)
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

                                    _detachCallback(func);
                                }
                            }
                        }
                    };

                    thread.Func = func;
                    _threads.Add(thread);

                    _attachCallback(func);

                    Logger.Debug($"Registering [Thread] to method: {method.Name}.");
                }
            }
            else
            {
                Logger.Error($"Unable to register [Thread] to method: {method.Name}, you need to delete parameters: [{string.Join(", ", methodParams.Select(x => x.ParameterType.Name))}]");
            }
        }

        public void StartThread(Func<Task> action) => _attachCallback(action);
        public void StopThread(Func<Task> action) => _detachCallback(action);

        public IEnumerable<Thread> GetThreads() => _threads.AsEnumerable();
    }
}
