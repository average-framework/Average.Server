using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.Attributes;
using Average.Shared.DataModels;
using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using static Average.Server.Services.RpcService;

namespace Average.Server.Handlers
{
    internal class InventoryHandler : IHandler
    {
        private readonly InventoryService _inventoryService;
        private readonly WorldService _worldService;

        public InventoryHandler(InventoryService inventoryService, WorldService worldService)
        {
            _inventoryService = inventoryService;
            _worldService = worldService;
        }

        [ServerEvent("character:character_created")]
        private async void OnCharacterCreated(Client client, string characterId)
        {
            var storage = new StorageData
            {
                StorageId = characterId,
                MaxWeight = InventoryService.DefaultMaxInventoryWeight,
                Type = StorageDataType.Player,
                Items = new()
            };

            await _inventoryService.Create(storage);

            Logger.Write("Inventory", $"Inventory created for {client.License} with storage id: %{characterId}% of type %{storage.Type}%",
                new Logger.TextColor(foreground: ConsoleColor.DarkYellow),
                new Logger.TextColor(foreground: ConsoleColor.Cyan));
        }

        [UICallback("window_ready")]
        private void OnWindowReady(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            Logger.Error("Window ready");

            _inventoryService.OnClientWindowInitialized(client);
        }

        [UICallback("frame_ready")]
        private async void OnFrameReady(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            if (args.TryGetValue("frame", out object frame))
            {
                if ((string)frame == "storage")
                {
                    _inventoryService.InitSlots(client, StorageDataType.Player, InventoryService.InventorySlotCount);
                    _inventoryService.InitSlots(client, StorageDataType.Chest, InventoryService.ChestSlotCount);
                    //_inventoryService.InitSlots(client, StorageDataType.VehicleInventory);

                    await BaseScript.Delay(1000);

                    var storage = _inventoryService.GetLocalStorage(client);
                    if (storage == null) return;

                    _inventoryService.LoadInventory(client, storage);
                    _inventoryService.SetTime(client, _worldService.World.Time);
                }
            }
        }

        [UICallback("storage/item_info")]
        private void OnItemInfo(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var slotId = int.Parse(args["slotId"].ToString());
            var storage = _inventoryService.GetLocalStorage(client);
            if (storage == null) return;

            var item = _inventoryService.GetItemOnSlot(slotId, storage);
            if (item == null) return;

            var info = _inventoryService.GetItemInfo(item.Name);

            cb(new
            {
                title = info.Title,
                description = info.Description,
                weight = (info.Weight * item.Count).ToString("0.00"),
                isSellable = info.IsSellable ? "Vendable" : "Invendable"
            });
        }

        [UICallback("storage/drop_slot")]
        private async void OnDropSlot(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var slotId = int.Parse(args["slotId"].ToString());
            var targetSlotId = int.Parse(args["targetSlotId"].ToString());
            var slotSourceType = (string)args["slotSourceType"];
            var slotTargetType = (string)args["slotTargetType"];

            Logger.Error("Drop slot target 1: " + string.Join(", ", slotId, slotSourceType, targetSlotId, slotTargetType));

            StorageData sourceStorage = null;
            StorageData destinationStorage = null;

            switch (slotSourceType)
            {
                case "inv":
                    sourceStorage = _inventoryService.GetLocalStorage(client);
                    break;
                case "chest":
                    sourceStorage = _inventoryService.GetData<StorageData>(client, "ChestData");
                    break;
            }

            switch (slotTargetType)
            {
                case "inv":
                    destinationStorage = _inventoryService.GetLocalStorage(client);
                    break;
                case "chest":
                    destinationStorage = _inventoryService.GetData<StorageData>(client, "ChestData");
                    break;
            }

            if (sourceStorage == null) return;
            if (destinationStorage == null) return;

            _inventoryService.MoveItemOnStorage(client, slotId, targetSlotId, sourceStorage, destinationStorage);
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

        [UICallback("storage/context_menu")]
        private async void OnContextMenu(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            Logger.Error("storage/context_menu triggered: " + string.Join(", ", args));

            var slotId = int.Parse(args["slotId"].ToString());
            var slotSourceType = (string)args["slotSourceType"];
            var eventName = (string)args["eventName"];

            if(slotSourceType == "inv")
            {
                var storage = _inventoryService.GetLocalStorage(client);
                if (storage == null) return;

                var item = _inventoryService.GetItemOnSlot(slotId, storage);
                if (item == null) return;

                await _inventoryService.OnStorageContextMenu(client, item.Name, slotId, eventName, storage);
            }
            else if(slotSourceType == "chest")
            {
                var storage = _inventoryService.GetData<StorageData>(client, "ChestData");
                if(storage == null) return;

                var item = _inventoryService.GetItemOnSlot(slotId, storage);
                if (item == null) return;

                await _inventoryService.OnStorageContextMenu(client, item.Name, slotId, eventName, storage);
            }
        }

        [UICallback("storage/split/result")]
        private void OnSplitResult(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var slotId = int.Parse(args["slotId"].ToString());
            var minValue = args["minValue"];
            var maxValue = args["maxValue"];
            var value = args["value"];
            var slotType = (string)args["slotType"];
            
            if(slotType == "inv")
            {
                var storage = _inventoryService.GetLocalStorage(client);
                if (storage == null) return;

                _inventoryService.OnSplitItem(client, slotId, minValue, maxValue, value, storage);
            }
            else if(slotType == "chest")
            {
                var storage = _inventoryService.GetData<StorageData>(client, "ChestData");
                if (storage == null) return;

                _inventoryService.OnSplitItem(client, slotId, minValue, maxValue, value, storage);
            }

            Logger.Debug("Split result: " + slotId + ", " + minValue + ", " + maxValue + ", " + value);
        }

        //[UICallback("storage/inv/split/close")]
        //private void OnInventorySplitMenuClosed(Client client, Dictionary<string, object> args, RpcCallback cb)
        //{

        //}

        //[UICallback("storage/veh/split/close")]
        //private void OnVehicleSplitMenuClosed(Client client, Dictionary<string, object> args, RpcCallback cb)
        //{

        //}

        //[UICallback("storage/chest/split/close")]
        //private void OnChestSplitMenuClosed(Client client, Dictionary<string, object> args, RpcCallback cb)
        //{

        //}
    }
}
