using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Interfaces;
using Average.Server.Services;
using Average.Shared.DataModels;
using System;

namespace Average.Server.Jobs
{
    [ServerJob]
    internal class CharacterJob : IServerJob
    {
        private readonly ClientService _clientService;
        private readonly CharacterService _characterService;
        private readonly UserService _userService;

        public Guid Id => Guid.NewGuid();
        public DateTime LastTriggered { get; set; }
        public TimeSpan Recurring => new TimeSpan(0, 0, 5);
        public JobState State { get; set; }
        public Func<bool> StartCondition => OnStartCondition;
        public Func<bool> StopCondition => OnStopCondition;

        public CharacterJob(ClientService clientService, UserService userService, CharacterService characterService)
        {
            _clientService = clientService;
            _characterService = characterService;
            _userService = userService;
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
            Logger.Info($"{GetType().Name} Started successfully.");
        }

        public void OnStop()
        {
            Dispose();
            Logger.Info($"[{GetType().Name}] Stopped successfully.");
        }

        public async void OnUpdate()
        {
            try
            {
                for (int i = 0; i < _clientService.Clients.Count; i++)
                {
                    var client = _clientService[i];
                    var characterData = await _characterService.Get(client);

                    if (characterData == null) continue;

                    var characterPosition = client.Player.Character.Position;
                    var characterHeading = client.Player.Character.Heading;

                    _characterService.UpdatePosition(characterData, new PositionData(characterPosition.X, characterPosition.Y, characterPosition.Z, characterHeading));
                }

                //Logger.Info($"[{GetType().Name}] job executed successfully.");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{GetType().Name}] Unable to execute this job. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }
    }
}
