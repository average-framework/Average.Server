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
        private async void CreateStorage(string license, float maxWeight, int type)
        {
            var storage = new StorageData();
            storage.StorageId = license;
            storage.Type = (StorageDataType)type;
            storage.MaxWeight = maxWeight;

            await _inventoryService.Create(storage);
        }

        [ClientCommand("storage.add_item")]
        private void AddItem(Client client, string itemName, int itemCount)
        {
            var storage = _inventoryService.GetLocalStorage(client);
            if (storage == null) return;

            Logger.Error($"Try to add item to client: {client.Name}, item: {itemName} count: {itemCount}");

            _inventoryService.AddItem(client, new StorageItemData(itemName, itemCount), storage);
        }

        [ClientCommand("storage.remove_item")]
        private void RemoveItem(Client client, string itemName, int itemCount)
        {

        }
    }
}
