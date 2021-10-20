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

        public InventoryHandler(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [ServerEvent("character:character_created")]
        private async void OnCharacterCreated(Client client, string characterId)
        {
            var storage = new StorageData
            {
                StorageId = characterId,
                MaxWeight = InventoryService.DefaultMaxInventoryWeight,
                Type = StorageDataType.PlayerInventory,
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
        private void OnFrameReady(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            if (args.TryGetValue("frame", out object frame))
            {
                if ((string)frame == "storage")
                {
                    _inventoryService.InitSlots(client, StorageDataType.PlayerInventory);
                    //_inventoryService.InitSlots(client, StorageDataType.VehicleInventory);
                    //_inventoryService.InitSlots(client, StorageDataType.Chest);
                }
            }
        }

        [UICallback("storage/inv/drop_slot")]
        private void OnInventoryDropSlot(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var slotId = int.Parse(args["slotId"].ToString());
            var targetSlotId = int.Parse(args["targetSlotId"].ToString());

            var storage = _inventoryService.GetLocalStorage(client);

            _inventoryService.SetItemOnSlot(client, storage, slotId, targetSlotId);
        }

        [UICallback("storage/veh/drop_slot")]
        private void OnVehicleDropSlot(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            //var slotId = int.Parse(args["slotId"].ToString());
            //var targetSlotId = int.Parse(args["targetSlotId"].ToString());

            //var storage = _inventoryService.GetLocalStorage(client);

            //_inventoryService.SetItemOnSlot(client, storage, slotId, targetSlotId);
        }

        [UICallback("storage/chest/drop_slot")]
        private void OnChestDropSlot(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            //var slotId = int.Parse(args["slotId"].ToString());
            //var targetSlotId = int.Parse(args["targetSlotId"].ToString());

            //var storage = _inventoryService.GetLocalStorage(client);

            //_inventoryService.SetItemOnSlot(client, storage, slotId, targetSlotId);
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

            var slotId = int.Parse(args["slotId"].ToString());
            var eventName = (string)args["eventName"];

            var storage = _inventoryService.GetLocalStorage(client);
            if (storage == null) return;

            var item = _inventoryService.GetItemOnSlot(slotId, storage);
            if(item == null) return;

            await _inventoryService.OnStorageContextMenu(client, item.Name, slotId, eventName, storage);
        }

        [UICallback("storage/veh/context_menu")]
        private async void OnVehicleContextMenu(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            //Logger.Error("storage/chest/context_menu triggered: " + string.Join(", ", args));

            //var itemName = (string)args["name"];
            //var slotId = int.Parse(args["slotId"].ToString());
            //var eventName = (string)args["eventName"];

            //var chestData = _inventoryService.GetData<StorageData>(client, "CurrentChestData");
            //if (chestData == null) return;

            //await _inventoryService.OnStorageContextMenu(client, itemName, slotId, eventName, chestData);
        }

        [UICallback("storage/chest/context_menu")]
        private async void OnChestContextMenu(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            //Logger.Error("storage/chest/context_menu triggered: " + string.Join(", ", args));

            //var itemName = (string)args["name"];
            //var slotId = int.Parse(args["slotId"].ToString());
            //var eventName = (string)args["eventName"];

            //var chestData = _inventoryService.GetData<StorageData>(client, "CurrentChestData");
            //if (chestData == null) return;

            //await _inventoryService.OnStorageContextMenu(client, itemName, slotId, eventName, chestData);
        }

        [UICallback("storage/inv/split/result")]
        private void OnInventorySplitResult(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var slotId = int.Parse(args["slotId"].ToString());
            var minValue = args["minValue"];
            var maxValue = args["maxValue"];
            var value = args["value"];

            var storage = _inventoryService.GetLocalStorage(client);
            if (storage == null) return;

            _inventoryService.OnSplitItem(client, slotId, minValue, maxValue, value, storage);

            Logger.Debug("Split result: " + slotId + ", " + minValue + ", " + maxValue + ", " + value);
        }

        [UICallback("storage/veh/split/result")]
        private void OnVehicleSplitResult(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            //var slotId = int.Parse(args["slotId"].ToString());
            //var minValue = args["minValue"];
            //var maxValue = args["maxValue"];
            //var value = args["value"];

            //var storage = _inventoryService.GetLocalStorage(client);
            //if (storage == null) return;

            //_inventoryService.OnSplitItem(client, slotId, minValue, maxValue, value, storage);

            //Logger.Debug("Split result: " + slotId + ", " + minValue + ", " + maxValue + ", " + value);
        }

        [UICallback("storage/chest/split/result")]
        private void OnChestSplitResult(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            //var slotId = int.Parse(args["slotId"].ToString());
            //var minValue = args["minValue"];
            //var maxValue = args["maxValue"];
            //var value = args["value"];

            //var storage = _inventoryService.GetLocalStorage(client);
            //if (storage == null) return;

            //_inventoryService.OnSplitItem(client, slotId, minValue, maxValue, value, storage);

            //Logger.Debug("Split result: " + slotId + ", " + minValue + ", " + maxValue + ", " + value);
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
