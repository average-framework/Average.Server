using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Interfaces;
using Average.Server.Services;
using Average.Shared.DataModels;
using CitizenFX.Core;

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
            Dispose();
            Logger.Info("CharacterJob Stopped successfully.");
        }

        public async void OnUpdate()
        {
            //if (_clientService.Clients.Count > 0)
            //{
            //    for (int i = 0; i < _clientService.Clients.Count; i++)
            //    {
            //        var client = _clientService[i];
            //        var characterData = await _characterService.Get(client, true);

            //        if (characterData == null) continue;

            //        var characterPosition = client.Player.Character.Position;
            //        var characterHeading = client.Player.Character.Heading;

            //        characterData.Position = new PositionData(characterPosition.X, characterPosition.Y, characterPosition.Z, characterHeading);
            //        _characterService.UpdateWithChilds(characterData);
            //    }
            //}
        }

        [Thread]
        private async Task Update2()
        {
            try
            {
                var characterData = await _characterService.Get("3fc1bd0189e46d4aaf3553818c43167b32db46e1", true);

                if (characterData == null) return;

                var characterPosition = new Vector3(new Random(Environment.TickCount + 1).Next(-3000, 3000), new Random(Environment.TickCount + 2).Next(-3000, 3000), new Random(Environment.TickCount + 3).Next(-3000, 3000));
                var characterHeading = (float)new Random().NextDouble() * 360f;

                characterData.Position = new PositionData(characterPosition.X, characterPosition.Y, characterPosition.Z, characterHeading);
                Logger.Error("Update: " + characterData.Position.ToJson());
                characterData.Data = new Dictionary<string, object>();
               
                await _characterService.Update(characterData);
            }
            catch (Exception ex)
            {
                Logger.Error("Enculer: " + ex.Message + "\n" + ex.StackTrace);
            }
            
            await BaseScript.Delay(1000);
        }
    }
}
