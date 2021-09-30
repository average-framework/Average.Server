using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Interfaces;
using Average.Server.Services;
using Average.Shared.DataModels;

namespace Average.Server.Jobs
{
    [ServerJob]
    internal class CharacterJob : IServerJob
    {
        private readonly ClientService _clientService;
        private readonly CharacterService _characterService;

        public Guid Id => Guid.NewGuid();
        public DateTime LastTriggered { get; set; }
        public TimeSpan Recurring => new TimeSpan(0, 0, 5);
        public JobState State { get; set; }
        public Func<bool> StartCondition => OnStartCondition;
        public Func<bool> StopCondition => OnStopCondition;

        public CharacterJob(ClientService clientService, CharacterService characterService)
        {
            _clientService = clientService;
            _characterService = characterService;
        }

        private bool OnStartCondition()
        {
            return _clientService.ClientCount > 0;
        }

        private bool OnStopCondition()
        {
            return _clientService.ClientCount == 0;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void OnStart()
        {
            Logger.Info("CharacterJob Started successfully.");
        }

        public void OnStop()
        {
            Logger.Info("CharacterJob Stopped successfully.");
        }

        public async void OnUpdate()
        {
            for (int i = 0; i < _clientService.ClientCount; i++)
            {
                var client = _clientService[i];
                var characterData = await _characterService.Get(client);
                var characterPosition = client.Player.Character.Position;
                var characterHeading = client.Player.Character.Heading;

                characterData.Position = new PositionData(characterPosition.X, characterPosition.Y, characterPosition.Z, characterHeading);
                _characterService.Update(characterData);
            }
        }
    }
}
