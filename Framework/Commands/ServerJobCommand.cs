using Average.Server.Framework.Attributes;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Services;

namespace Average.Server.Framework.Commands
{
    internal class ServerJobCommand : ICommand
    {
        private readonly ServerJobService _serverJobService;

        public ServerJobCommand(ServerJobService serverJobService)
        {
            _serverJobService = serverJobService;
        }

        [ServerCommand("svjob.update")]
        private void OnSaveAll()
        {
            foreach (var job in _serverJobService.Jobs)
            {
                job.OnUpdate();
            }
        }

        [ServerCommand("svjob.start_all")]
        private void OnStartAll()
        {
            foreach (var job in _serverJobService.Jobs)
            {
                job.OnStart();
            }
        }

        [ServerCommand("svjob.stop_all")]
        private void OnStopAll()
        {
            foreach (var job in _serverJobService.Jobs)
            {
                job.OnStop();
            }
        }
    }
}
