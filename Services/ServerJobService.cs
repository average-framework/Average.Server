using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Interfaces;
using CitizenFX.Core;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class ServerJobService : IService
    {
        private readonly IContainer _container;

        private readonly List<Tuple<IServerJob, ServerJobAttribute>> _jobs = new();

        public ServerJobService(IContainer container)
        {
            _container = container;
        }

        [Thread]
        private async Task Update()
        {
            for (int i = 0; i < _jobs.Count; i++)
            {
                var tuple = _jobs[i];

                if (tuple.Item1.State == JobState.Stopped)
                {
                    var job = tuple.Item1;

                    if (job.StartCondition.Invoke())
                    {
                        OnStartJob(job);
                        OnUpdateJob(job);
                    }

                    _jobs[i] = new Tuple<IServerJob, ServerJobAttribute>(job, tuple.Item2);
                }

                if (tuple.Item1.State == JobState.Started)
                {
                    var job = tuple.Item1;

                    if (job.StopCondition.Invoke())
                    {
                        OnStopJob(job);
                    }

                    if (job.LastTriggered + job.Recurring >= DateTime.Now)
                    {
                        continue;
                    }

                    OnUpdateJob(job);
                }
            }

            await BaseScript.Delay(1000);
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
                    var service = _container.Resolve(type);
                    if (service == null) continue;

                    if (service is IServerJob @job)
                    {
                        var attr = type.GetCustomAttribute<ServerJobAttribute>();
                        if (attr == null) continue;

                        RegisterInternalJob(attr, @job);
                    }
                }
            }
        }

        private void OnStartJob(IServerJob job)
        {
            try
            {
                job.State = JobState.Started;
                job.OnStart();
            }
            catch (Exception ex)
            {
                Logger.Error($"Unable to start job {job.GetType().Name}. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }

        private void OnStopJob(IServerJob job)
        {
            try
            {
                job.State = JobState.Stopped;
                job.OnStop();
            }
            catch (Exception ex)
            {
                Logger.Error($"Unable to stop job {job.GetType().Name}. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }

        private void OnUpdateJob(IServerJob job)
        {
            try
            {
                job.OnUpdate();
                job.LastTriggered = DateTime.Now;
            }
            catch (Exception ex)
            {
                Logger.Error($"Unable to update job {job.GetType().Name}. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }

        private void RegisterInternalJob(ServerJobAttribute attr, IServerJob classObj)
        {
            _jobs.Add(new Tuple<IServerJob, ServerJobAttribute>(classObj, attr));
            Logger.Debug($"Registering [ServerJob] of type: {classObj.GetType().Name} with id {classObj.Id}.");
        }
    }
}
