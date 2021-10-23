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
        private async void OnInventoryDropSlot(Client client, Dictionary<string, object> args, RpcCallback cb)
        {
            var slotId = int.Parse(args["slotId"].ToString());
            var targetSlotId = int.Parse(args["targetSlotId"].ToString());
            var slotSourceType = (string)args["slotSourceType"];
            var slotTargetType = (string)args["slotTargetType"];

            Logger.Error("Drop slot target 1: " + string.Join(", ", slotId, slotSourceType, targetSlotId, slotTargetType));

            // Récupère l'instance de l'item dans l'inventaire
            //var item = _inventoryService.GetItemOnSlot(slotId, storage);
            //if (item == null) return;

            // Execute une action différente en fonction de la destination de l'item (inventaire, coffre, etc)
            if(slotSourceType == "inv" && slotTargetType == "inv")
            {
                // Inventaire -> Inventaire

                if (slotId == targetSlotId) return;

                // Récupère l'inventaire du joueur
                var storage = _inventoryService.GetLocalStorage(client);
                if (storage == null) return;

                Logger.Error("Inventory 1");
                // On déplace l'item sur un slot définis
                _inventoryService.SetItemOnSlot(client, storage, slotId, targetSlotId);
            }
            else if (slotSourceType == "chest" && slotTargetType == "chest")
            {
                // Coffre -> Coffre

                if (slotId == targetSlotId) return;

                Logger.Error("Chest 1");
                var chestStorage = _inventoryService.GetData<StorageData>(client, "ChestData");
                if (chestStorage == null) return;
                Logger.Error("Chest 2");

                // On déplace l'item sur un slot définis
                _inventoryService.SetItemOnSlot(client, chestStorage, slotId, targetSlotId);
            }
            else if (slotSourceType == "inv" && slotTargetType == "chest")
            {
                // Inventaire -> Coffre

                // Récupère l'inventaire du joueur
                var storage = _inventoryService.GetLocalStorage(client);
                if (storage == null) return;

                // Récupèrte l'item dans l'inventaire du joueur
                var item = _inventoryService.GetItemOnSlot(slotId, storage);
                if (item == null) return;

                Logger.Error("Inv -> Chest 1");
                var chestStorage = _inventoryService.GetData<StorageData>(client, "ChestData");
                if (chestStorage == null) return;
                Logger.Error("Inv -> Chest 2: " + chestStorage.ToJson());

                // Besoin de créer une copie de l'item avant de l'ajouter dans le coffre
                // pour éviter que l'instance de l'item dans le coffre soit la même que celle de l'inventaire
                var newItem = new StorageItemData(item.Name, item.Count);
                newItem.SlotId = targetSlotId;

                // Créer une copie des données de l'item (sans l'instance) et les copies sur newItem
                var newDictionary = item.Data.ToDictionary(entry => entry.Key, entry => entry.Value);
                newItem.Data = newDictionary;

                if(_inventoryService.IsSlotAvailable(newItem.SlotId, chestStorage))
                {
                    // On vérifie que l'inventaire à assez de place pour recevoir l'item provenant du coffre
                    if (!_inventoryService.HasFreeSpaceForWeight(newItem, chestStorage))
                    {
                        Logger.Error("Pas assez de place dans le coffre pour ajouter l'item de l'inventaire: " + newItem.Name + ", " + newItem.Count);
                        return;
                    }

                    // Ajoute l'item dans le coffre sur un slot spécifique
                    _inventoryService.AddItem(client, newItem, chestStorage, true, targetSlotId);

                    // Supprime l'item de l'inventaire
                    _inventoryService.RemoveItemOnSlot(client, storage, slotId);

                    // Met à jour l'inventaire et le coffre dans la base de donnée
                    _inventoryService.Update(storage);
                    _inventoryService.Update(chestStorage);
                }
                else
                {
                    Logger.Error("Test 1");

                    // Récupère l'instance de l'item dans le coffre
                    var itemInstance = chestStorage.Items.Find(x => x.SlotId == targetSlotId);
                    if (itemInstance == null) return;

                    if (itemInstance.Name == newItem.Name)
                    {
                        // Les items sont identique, on les stack
                        Logger.Error("Test 2: " + itemInstance.Name + ", " + itemInstance.Count + ", " + newItem.Name + ", " + newItem.Count);

                        // On vérifie que l'inventaire à assez de place pour recevoir l'item provenant du coffre
                        if (!_inventoryService.HasFreeSpaceForWeight(newItem, chestStorage))
                        {
                            Logger.Error("Pas assez de place dans le coffre pour stack l'item de l'inventaire: " + itemInstance.Name + ", " + itemInstance.Count);
                            return;
                        }

                        // Besoin de stack l'item dans le coffre
                        _inventoryService.StackItemOnSlot(client, chestStorage, newItem, itemInstance);

                        // Supprime l'item de l'inventaire
                        _inventoryService.RemoveItemOnSlot(client, storage, slotId);

                        // Met à jour l'inventaire et le coffre dans la base de donnée
                        _inventoryService.Update(storage);
                        _inventoryService.Update(chestStorage);
                    }
                    else
                    {
                        // Les items sont différent, on les alternes

                        Logger.Error("Alternate");

                        var sourceCopy = new StorageItemData(item.Name, item.Count);
                        sourceCopy.SlotId = item.SlotId;
                        var sourceDictionary = item.Data.ToDictionary(entry => entry.Key, entry => entry.Value);
                        sourceCopy.Data = sourceDictionary;

                        var destinationCopy = new StorageItemData(itemInstance.Name, itemInstance.Count);
                        destinationCopy.SlotId = itemInstance.SlotId;
                        var destinationDictionary = itemInstance.Data.ToDictionary(entry => entry.Key, entry => entry.Value);
                        destinationCopy.Data = destinationDictionary;

                        // On vérifie que l'inventaire à assez de place pour recevoir l'item provenant du coffre
                        if (!_inventoryService.HasFreeSpaceForWeight(destinationCopy, storage))
                        {
                            Logger.Error("Pas assez de place dans l'inventaire pour recevoir l'item du coffre: " + destinationCopy.Name + ", " + destinationCopy.Count);
                            return;
                        }

                        // On vérifie que le coffre à assez de place pour recevoir l'item provenant de l'inventaire
                        if (!_inventoryService.HasFreeSpaceForWeight(sourceCopy, chestStorage))
                        {
                            Logger.Error("Pas assez de place dans le coffre pour recevoir l'item de l'inventaire: " + sourceCopy.Name + ", " + sourceCopy.Count);
                            return;
                        }

                        // On supprime l'item de l'inventaire ce trouvant sur le slotId définis avant d'ajouter le nouvelle item
                        _inventoryService.RemoveItemOnSlot(client, storage, slotId);

                        // On supprime l'item du coffre ce trouvant sur le slotId définis avant d'ajouter le nouvelle item
                        _inventoryService.RemoveItemOnSlot(client, chestStorage, targetSlotId);

                        Logger.Error("Informations: " + slotId + ", " + targetSlotId + "\n" + sourceCopy.ToJson(Newtonsoft.Json.Formatting.Indented) + "\n" + destinationCopy.ToJson(Newtonsoft.Json.Formatting.Indented));

                        // Ajoute l'item du coffre dans l'inventaire
                        _inventoryService.AddItem(client, destinationCopy, storage, true, slotId);

                        // Ajoute l'item de l'inventaire dans le coffre
                        _inventoryService.AddItem(client, sourceCopy, chestStorage, true, targetSlotId);
                    }
                }
            }
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
