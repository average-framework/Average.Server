using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Interfaces;
using Average.Server.Services;
using Average.Shared.DataModels;
using System;

namespace Average.Server.Jobs
{
    [ServerJob]
    internal class InventoryJob : IServerJob
    {
        private readonly ClientService _clientService;
        private readonly CharacterService _characterService;
        private readonly InventoryService _inventoryService;

        public Guid Id => Guid.NewGuid();
        public DateTime LastTriggered { get; set; }
        public TimeSpan Recurring => new TimeSpan(0, 0, 5);
        public JobState State { get; set; }
        public Func<bool> StartCondition => OnStartCondition;
        public Func<bool> StopCondition => OnStopCondition;

        public InventoryJob(ClientService clientService, CharacterService characterService, InventoryService inventoryService)
        {
            _clientService = clientService;
            _characterService = characterService;
            _inventoryService = inventoryService;
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

                    var inventoryData = _inventoryService.GetLocalStorage(client);
                    if (inventoryData == null) continue;

                    Logger.Debug("inventory data: " + inventoryData.ToJson());

                    await _inventoryService.Update(inventoryData);
                }

                Logger.Info($"[{GetType().Name}] job executed successfully.");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{GetType().Name}] Unable to execute this job. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }
    }
}
