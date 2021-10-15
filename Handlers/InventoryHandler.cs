using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.Attributes;
using Average.Shared.DataModels;
using System;
using System.Collections.Generic;
using static Average.Server.Services.RpcService;

namespace Average.Server.Handlers
{
    internal class InventoryHandler : IHandler
    {
        private readonly InventoryService _inventoryService;
        private readonly ClientService _clientService;

        public InventoryHandler(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [ServerEvent("character:character_created")]
        private async void OnCharacterCreated(string clientLicense, string characterId)
        {
            var storage = new StorageData
            {
                StorageId = characterId,
                MaxWeight = InventoryService.DefaultMaxInventoryWeight,
                Type = StorageDataType.PlayerInventory,
                Items = new()
            };

            await _inventoryService.Create(storage);

            Logger.Write("Inventory", $"Inventory created for {clientLicense} with storage id: %{characterId}% of type %{storage.Type}%",
                new Logger.TextColor(foreground: ConsoleColor.DarkYellow),
                new Logger.TextColor(foreground: ConsoleColor.Cyan));
        }

        [UICallback("storage/keydown")]
        private void OnKeydown(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var key = int.Parse((string)args["key"]);

            if (_inventoryService.GetData<bool>(client, "IsOpen") && key == 27)
            {
                _inventoryService.Close(client);
            }
        }

        [UICallback("storage/close")]
        private void OnClose(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            Logger.Error("storage:close triggered: " + string.Join(", ", args));
            _inventoryService.Close(client);
        }

        [UICallback("storage/inv/context_menu")]
        private async void OnInventoryContextMenu(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            Logger.Error("storage/inv/context_menu triggered: " + string.Join(", ", args));

            var itemName = (string)args["name"];
            var itemId = (string)args["tempId"];
            var eventName = (string)args["eventName"];

            var storage = _inventoryService.GetLocalStorage(client);
            if (storage == null) return;

            await _inventoryService.OnStorageContextMenu(client, itemName, itemId, eventName, storage);
        }

        [UICallback("storage/chest/context_menu")]
        private async void OnChestContextMenu(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            Logger.Error("storage/chest/context_menu triggered: " + string.Join(", ", args));

            var itemName = (string)args["name"];
            var itemId = (string)args["tempId"];
            var eventName = (string)args["eventName"];

            var chestData = _inventoryService.GetData<StorageData>(client, "CurrentChestData");
            if (chestData == null) return;

            await _inventoryService.OnStorageContextMenu(client, itemName, itemId, eventName, chestData);
        }

        [UICallback("storage/inv/input_count")]
        private void OnInventoryInputCount(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            Logger.Error("storage/inv/input_count triggered: " + string.Join(", ", args));

            var val = (string)args["value"];

            var storage = _inventoryService.GetLocalStorage(client);
            if (storage == null) return;

            _inventoryService.OnStorageInputCount(client, storage, val);
        }

        [UICallback("storage/chest/input_count")]
        private void OnChestInputCount(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            Logger.Error("storage/chest/input_count triggered: " + string.Join(", ", args));

            var val = (string)args["value"];

            var chestData = _inventoryService.GetData<StorageData>(client, "CurrentChestData");
            if (chestData == null) return;

            _inventoryService.OnStorageInputCount(client, chestData, val);
        }
    }
}
