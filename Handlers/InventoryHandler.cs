﻿using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.Attributes;
using Average.Shared.DataModels;
using CitizenFX.Core;
using System;
using System.Collections.Generic;
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

        [UICallback("storage/inv/item_info")]
        private void OnInventoryItemInfo(Client client, Dictionary<string, object> args, RpcCallback cb)
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

        [UICallback("storage/inv/drop_slot")]
        private void OnInventoryDropSlot(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var slotId = int.Parse(args["slotId"].ToString());
            var targetSlotId = int.Parse(args["targetSlotId"].ToString());

            Logger.Debug("Drop slot target 1: " + string.Join(", ", slotId, targetSlotId));

            var storage = _inventoryService.GetLocalStorage(client);
            if (storage == null) return;

            _inventoryService.SetItemOnSlot(client, storage, slotId, targetSlotId);
        }


        //[UICallback("storage/chest/drop_slot")]
        //private void OnChestDropSlot(Client client, Dictionary<string, object> args, RpcCallback cb)
        //{
        //    var slotId = int.Parse(args["slotId"].ToString());
        //    var targetSlotId = int.Parse(args["targetSlotId"].ToString());
        //    var slotSourceType = (string)args["slotSourceType"];

        //    Logger.Debug("Drop slot target 2: " + string.Join(", ", slotId, targetSlotId, slotSourceType));

        //    var storage = _inventoryService.GetLocalStorage(client);
        //    if (storage == null) return;

        //    var chestStorage = _inventoryService.GetData<StorageData>(client, "ChestData");
        //    if (chestStorage == null) return;

        //    // Inventory Item
        //    var item = _inventoryService.GetItemOnSlot(slotId, storage);
        //    if(item == null) return;

        //    switch (slotSourceType)
        //    {
        //        case "inv":
        //            //_inventoryService.RemoveItemOnSlot(client, storage, slotId);
        //            break;
        //        case "chest":
        //            break;
        //    }

        //    _inventoryService.AddItem(client, item, chestStorage);
        //}

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
