using SDK.Shared.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SDK.Server.Diagnostics;

namespace Average.Server.Managers
{
    public class ThreadManager : InternalPlugin, IThreadManager
    {
        private static readonly List<Thread> _threads = new List<Thread>();

        private static Action<Func<Task>> attachCallback;
        private static Action<Func<Task>> detachCallback;

        public ThreadManager()
        {
            attachCallback = Main.attachCallback;
            detachCallback = Main.detachCallback;
        }

        internal static void RegisterInternalThread(MethodInfo method, ThreadAttribute threadAttr, object classObj)
        {
            var methodParams = method.GetParameters();

            if (methodParams.Count() == 0)
            {
                if (threadAttr != null)
                {
                    var thread = new Thread(method, threadAttr.StartDelay);
                    Func<Task>? func = null;

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

                                    detachCallback(func);
                                }
                            }
                        }
                    };

                    thread.Func = func;
                    _threads.Add(thread);

                    attachCallback(func);

                    Log.Debug($"Registering [Thread] attribute to method: {method.Name}.");
                }
            }
            else
            {
                Log.Error($"Unable to register [Thread] attribute: {method.Name}, you need to delete parameters: [{string.Join(", ", methodParams.Select(x => x.ParameterType.Name))}]");
            }
        }

        public void StartThread(Func<Task> action) => attachCallback(action);

        public void StopThread(Func<Task> action) => detachCallback(action);

        public void UnregisterThread(string methodName)
        {
            var thread = _threads.Find(x => x.Method.Name == methodName);

            if (thread != null)
            {
                detachCallback(thread.Func);
                _threads.Remove(thread);

                Log.Debug($"Unregistering [Thread] attribute to method: {methodName}.");
            }
            else
            {
                Log.Debug($"Unable to unregistering [Thread] attribute from method: {methodName}.");
            }
        }

        public IEnumerable<Thread> GetThreads() => _threads.AsEnumerable();
    }
}
