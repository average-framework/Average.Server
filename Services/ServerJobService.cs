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
        private readonly List<IServerJob> _jobs = new List<IServerJob>();

        private const int Delay = 1000;

        public List<IServerJob> Jobs => _jobs;

        public ServerJobService(IContainer container)
        {
            _container = container;

            Logger.Write("ServerJobService", "Initialized successfully");
        }

        [Thread]
        private async Task Update()
        {
            for (int i = 0; i < _jobs.Count; i++)
            {
                var job = _jobs[i];

                if (job.State == JobState.Stopped)
                {
                    if (job.StartCondition.Invoke())
                    {
                        OnStartJob(job);
                        OnUpdateJob(job);
                    }
                }

                if (job.State == JobState.Started)
                {
                    if (job.StopCondition.Invoke())
                    {
                        OnStopJob(job);
                        continue;
                    }

                    if (job.LastTriggered + job.Recurring >= DateTime.Now)
                    {
                        continue;
                    }

                    OnUpdateJob(job);
                }
            }

            await BaseScript.Delay(Delay);
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

                        RegisterInternalJob(@job);
                    }
                }
            }
        }

        private void OnStartJob(IServerJob job)
        {
            try
            {
                job.OnStart();
                job.State = JobState.Started;
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
                job.OnStop();
                job.State = JobState.Stopped;
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

        private void RegisterInternalJob(IServerJob job)
        {
            _jobs.Add(job);
            Logger.Write("ServerJob", $"Registering [ServerJob] of type: {job.GetType().Name} with id %{job.Id}%.", new Logger.TextColor(foreground: ConsoleColor.DarkYellow));
        }
    }
}
