using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class JobService : IService
    {
        private readonly CharacterService _characterService;

        public JobService(CharacterService characterService)
        {
            _characterService = characterService;

            Logger.Write("JobService", "Initialized successfully");
        }

        internal async Task SetJob(Client client, Client target, string jobName, int jobLevel)
        {
            var characterData = await _characterService.Get(target);

            if (characterData != null)
            {
                characterData.Job.Name = jobName;
                characterData.Job.Level = jobLevel;
                _characterService.Update(characterData);

                Logger.Warn($"[JobService] Set job {jobName} for {target.Name} by {client.Name}.");
            }
            else
            {
                Logger.Error($"[JobService] Unable to set job to target: {target.License}");
            }
        }
    }
}
