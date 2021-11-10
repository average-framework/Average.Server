using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.DataModels;

namespace Average.Server.Framework.Commands
{
    internal class InventoryCommand : ICommand
    {
        private readonly InventoryService _inventoryService;

        public InventoryCommand(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [ServerCommand("storage.create")]
        private async void CreateStorage(string storageId, float maxWeight, int type)
        {
            var storage = new StorageData();
            storage.StorageId = storageId;
            storage.Type = (StorageDataType)type;
            storage.MaxWeight = maxWeight;

            await _inventoryService.Create(storage);
        }
    }
}
