using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Models;
using Average.Server.Repositories;
using Average.Shared.DataModels;
using Average.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Average.Server.Services.InputService;

namespace Average.Server.Services
{
    internal class InventoryService : IService
    {
        private readonly RpcService _rpcService;
        private readonly InventoryRepository _repository;
        private readonly UIService _uiService;
        private readonly InputService _inputService;
        private readonly WorldService _worldService;

        private readonly List<StorageItemInfo> _items = new();
        private readonly Dictionary<string, Dictionary<string, object>> _clients = new();

        public const double DefaultMaxInventoryWeight = 20.0;
        public const double DefaultMaxChestWeight = 100.0;

        private const int SlotCount = 20;

        public InventoryService(WorldService worldService, RpcService rpcService, InventoryRepository repository, UIService uiService, InputService inputService)
        {
            _worldService = worldService;
            _rpcService = rpcService;
            _repository = repository;
            _uiService = uiService;
            _inputService = inputService;

            // Inputs
            _inputService.RegisterKey(new Input((Control)0xC1989F95,
            condition: (client) =>
            {
                return true;
            },
            onStateChanged: (client, state) =>
            {
                Logger.Debug($"Client {client.Name} can open/close inventory");
            },
            onKeyReleased: (client) =>
            {
                Open(client);
                Logger.Debug($"Client {client.Name} open inventory");
            }));

            Logger.Write("InventoryService", "Initialized successfully");
        }

        internal async void OnClientInitialized(Client client, string characterId)
        {
            if (!_clients.ContainsKey(client.License))
            {
                var dict = new Dictionary<string, object>();
                var storage = await Get(characterId);

                dict.Add("Storage", storage ?? new StorageData
                {
                    //StorageId = client.License,
                    StorageId = characterId,
                    MaxWeight = DefaultMaxInventoryWeight,
                    Type = StorageDataType.PlayerInventory
                });
                //dict.Add("CharacterId", characterId);
                dict.Add("IsOpen", false);

                _clients.Add(client.License, dict);
            }
        }

        internal void OnClientWindowInitialized(Client client)
        {
            _uiService.LoadFrame(client, "storage");
            _uiService.SetZIndex(client, "storage", 90000);
        }

        internal void RegisterItem(StorageItemInfo itemInfo)
        {
            _items.Add(itemInfo);
        }

        internal void InitSlots(Client client, StorageDataType storageType)
        {
            var type = GetStorageTypeString(storageType);

            _uiService.SendMessage(client, "storage", "init", new
            {
                slotCount = SlotCount,

                type
            });
        }

        internal async Task<bool> SaveInventory(Client client)
        {
            var storage = GetLocalStorage(client);
            if (storage == null) return false;

            return await Update(storage);
        }

        private Dictionary<string, object> GetLocal(Client client)
        {
            if (_clients.ContainsKey(client.License))
            {
                return _clients[client.License];
            }
            else
            {
                return null;
            }
        }

        internal StorageData? GetLocalStorage(Client client)
        {
            var cache = GetLocal(client);
            return cache != null ? cache["Storage"] as StorageData : null;
        }

        internal void SetLocalStorage(Client client, StorageData storageData)
        {
            var cache = GetLocal(client);
            if (cache == null) return;
            cache["Storage"] = storageData;
        }

        internal T? GetData<T>(Client client, string key)
        {
            if (_clients.ContainsKey(client.License) && _clients[client.License].ContainsKey(key))
            {
                return (T)_clients[client.License][key];
            }
            else
            {
                return default;
            }
        }

        internal void SetData(Client client, string key, object value)
        {
            if (_clients.ContainsKey(client.License))
            {
                if (!_clients[client.License].ContainsKey(key))
                {
                    _clients[client.License].Add(key, value);
                }
                else
                {
                    _clients[client.License][key] = value;
                }
            }
        }

        private async Task<StorageData> LoadStorage(Client client, string storageId)
        {
            var storage = await Get(storageId);

            foreach (var item in storage.Items)
            {
                var info = GetItemInfo(item.Name);

                switch (storage.Type)
                {
                    case StorageDataType.PlayerInventory:
                        info?.OnInventoryLoading?.Invoke();
                        break;
                    case StorageDataType.VehicleInventory:
                    case StorageDataType.Chest:
                        info?.OnChestLoading?.Invoke();
                        SetData(client, "CurrentChestData", storageId);
                        break;
                }
            }

            SetLocalStorage(client, storage);
            return storage;
        }

        internal async void Close(Client client)
        {
            if (GetData<bool>(client, "IsOpen"))
            {
                SetData(client, "IsOpen", false);

                if (await _rpcService.NativeCall<bool>(client, 0x4A123E85D7C4CA0B, PostEffect.PauseMenuIn))
                {
                    _rpcService.NativeCall(client, 0xB4FD7446BAB2F394, PostEffect.PauseMenuIn);
                }

                _uiService.SendMessage(client, "storage", "close");
                _uiService.Unfocus(client);
            }
        }

        internal async void Open(Client client)
        {
            if (!GetData<bool>(client, "IsOpen"))
            {
                SetData(client, "IsOpen", true);

                if (!await _rpcService.NativeCall<bool>(client, 0x4A123E85D7C4CA0B, PostEffect.PauseMenuIn))
                {
                    _rpcService.NativeCall(client, 0x4102732DF6B4005F, PostEffect.PauseMenuIn);
                }

                _uiService.SendMessage(client, "storage", "open");
                _uiService.Focus(client);
            }
        }

        internal bool HaveItem(string itemName, StorageData storageData)
        {
            return storageData.Items.Exists(x => x.Name == itemName);
        }

        internal bool HaveItemCount(string itemName, int itemCount, StorageData storageData)
        {
            var item = storageData.Items.Find(x => x.Name == itemName);
            return item != null && item.Count >= itemCount;
        }

        internal StorageItemData GetItemFromStorage(int slotId, StorageData storageData)
        {
            return storageData.Items.Find(x => x.SlotId == slotId);
        }

        internal StorageItemInfo GetItemInfo(string itemName)
        {
            return _items.Find(x => x.Name == itemName);
        }

        internal double CalculateWeight(StorageData storageData)
        {
            var weight = 0d;
            storageData.Items.ForEach(x => weight += GetItemInfo(x.Name).Weight * x.Count);
            return weight;
        }

        internal bool HasFreeSpaceForWeight(double itemWeight, StorageData storageData)
        {
            return CalculateWeight(storageData) + itemWeight <= storageData.MaxWeight;
        }

        internal bool HasFreeSpace(StorageData storageData)
        {
            return CalculateWeight(storageData) <= storageData.MaxWeight;
        }

        internal bool IsSlotAvailable(int slotId, StorageData storageData)
        {
            return !storageData.Items.Exists(x => x.SlotId == slotId);
        }

        internal bool ItemExistsByName(string itemName, StorageData storageData)
        {
            return storageData.Items.Exists(x => x.Name == itemName);
        }

        internal StorageItemData GetItemOnSlot(int slotId, StorageData storageData)
        {
            return storageData.Items.Find(x => x.SlotId == slotId);
        }

        internal StorageItemData GetItemByName(string itemName, StorageData storageData)
        {
            return storageData.Items.Find(x => x.Name == itemName);
        }

        internal async Task Reset(StorageData storageData)
        {
            storageData = new StorageData();
            await Update(storageData);
        }

        internal bool IsSlotAvailable(Client client, int slotIndex)
        {
            var storage = GetLocalStorage(client);
            if (storage == null) return false;

            return storage.Items.Exists(x => x.SlotId == slotIndex);
        }

        internal bool IsSlotExistsWithItemName(Client client, string itemName)
        {
            var storage = GetLocalStorage(client);
            if (storage == null) return false;

            return storage.Items.Exists(x => x.Name == itemName);
        }

        internal int GetAvailableSlot(Client client)
        {
            var storage = GetLocalStorage(client);
            if (storage == null) return -1;

            for (int i = 0; i < SlotCount; i++)
            {
                if (!storage.Items.Exists(x => x.SlotId == i))
                {
                    return i;
                }
            }

            // No available slot
            return -1;
        }

        private string GetStorageTypeString(StorageDataType storageType)
        {
            var type = "";

            switch (storageType)
            {
                case StorageDataType.PlayerInventory:
                    type = "inv";
                    break;
                case StorageDataType.VehicleInventory:
                    type = "veh";
                    break;
                case StorageDataType.Chest:
                    type = "chest";
                    break;
            }

            return type;
        }

        internal void ShowSplitMenu(Client client, StorageDataType storageType, StorageItemInfo itemInfo, int slotId, object minValue, object maxValue, object defaultValue)
        {
            var type = GetStorageTypeString(storageType);

            _uiService.SendMessage(client, "storage", "showInvSplitMenu", new
            {
                slotId,
                title = itemInfo.Text,
                img = itemInfo.Img,
                defaultValue,
                minValue,
                maxValue,

                type
            });
        }

        internal void OnSplitItem(Client client, int slotId, object minValue, object maxValue, object value, StorageData storage)
        {
            SplitItem(client, slotId, minValue, maxValue, value, storage);
        }

        internal void SplitItem(Client client, int slotId, object minValue, object maxValue, object value, StorageData storageData)
        {
            var item = GetItemOnSlot(slotId, storageData);
            if (item == null) return;

            var info = GetItemInfo(item.Name);
            minValue = Convert.ChangeType(minValue, info.SplitValueType);
            maxValue = Convert.ChangeType(maxValue, info.SplitValueType);
            value = Convert.ChangeType(value, info.SplitValueType);

            Logger.Debug("Values: " + minValue + ", " + maxValue + ", " + value + ", " + item.Count);

            switch (value)
            {
                case int convertedValue:
                    Logger.Debug("item value type: " + convertedValue.GetType() + ", " + convertedValue);

                    if(item.Count == (int)minValue) return;

                    var valResult = 0;

                    Logger.Debug("value if: " + (convertedValue == (int)minValue) + ", " + (convertedValue == (int)maxValue));

                    if (convertedValue == (int)minValue || convertedValue == (int)maxValue)
                    {
                        // 15 - 15 = 0
                        // 15 - (15 - 1) = f:14 / s:1
                        valResult = (int)maxValue - 1;
                    }
                    else
                    {
                        //valResult = (int)maxValue - (int)convertedValue;
                        valResult = convertedValue;
                    }

                    Logger.Debug("Split ???: " + (info.OnSplit != null));

                    if (info.OnSplit != null)
                    {
                        // Split custom
                        // Application des modifications sur l'item après le split
                        info.OnSplit.Invoke(item, valResult, StorageItemInfo.SplitType.BaseItem);
                    }
                    else
                    {
                        // Split par défaut
                        item.Count = valResult;

                        Logger.Debug("item slot: " + item.SlotId + ", result: " + valResult);
                    }

                    // Appel l'action par defaut
                    Logger.Debug("value: " + item.Count);

                    // Met à jour l'affichage du premier item
                    UpdateSlotRender(client, item, info, storageData);

                    Logger.Debug("Slot count: " + item.Name + ", " + item.SlotId);

                    var newSlotId = GetAvailableSlot(client);
                    var newItem = new StorageItemData(item.Name, (int)maxValue - valResult);
                    newItem.SlotId = newSlotId;

                    var newDictionary = item.Data.ToDictionary(entry => entry.Key, entry => entry.Value);
                    newItem.Data = newDictionary;

                    Logger.Debug("before: " + item.ToJson(Newtonsoft.Json.Formatting.Indented) + "\n" + newItem.ToJson(Newtonsoft.Json.Formatting.Indented));

                    storageData = GetLocalStorage(client);
                    if (storageData == null) return;

                    storageData.Items.Add(newItem);
                    SetItemOnEmptySlot(client, storageData, newItem);
                    break;
                case decimal convertedValue:
                    Logger.Debug("item value type: " + convertedValue.GetType() + ", " + convertedValue);

                    var canSplit = info.SplitCondition != null && info.SplitCondition.Invoke(item);
                    if (!canSplit) return;

                    var valDecResult = 0m;

                    Logger.Debug("value if: " + (convertedValue == (decimal)minValue) + ", " + (convertedValue == (decimal)maxValue));

                    if (convertedValue == (decimal)minValue || convertedValue == (decimal)maxValue)
                    {
                        // 15 - 15 = 0
                        // 15 - (15 - 1) = f:14 / s:1
                        valDecResult = (decimal)maxValue - 1;
                    }
                    else
                    {
                        //valResult = (decimal)maxValue - (decimal)convertedValue;
                        valDecResult = convertedValue;
                    }

                    Logger.Debug("Split ???: " + (info.OnSplit != null));

                    if (info.OnSplit != null)
                    {
                        Logger.Debug("before split: " + item.Data["cash"]);
                        info.OnSplit.Invoke(item, valDecResult, StorageItemInfo.SplitType.BaseItem);
                        Logger.Debug("after split: " + item.Data["cash"]);
                    }

                    // Appel l'action par defaut
                    Logger.Debug("value: " + item.Count);

                    // Met à jour l'affichage du premier item
                    UpdateSlotRender(client, item, info, storageData);

                    Logger.Debug("Slot count: " + item.Name + ", " + item.SlotId);

                    newSlotId = GetAvailableSlot(client);
                    newItem = new StorageItemData(item.Name, 1);
                    newItem.SlotId = newSlotId;

                    // Besoin de copier les données, sinon les données du base item et du target item sont encore lier
                    newDictionary = item.Data.ToDictionary(entry => entry.Key, entry => entry.Value);

                    newItem.Data = newDictionary;
                    Logger.Debug("before split: " + item.Data["cash"] + ", " + newItem.Data["cash"]);
                    info.OnSplit.Invoke(newItem, valDecResult, StorageItemInfo.SplitType.TargetItem);
                    Logger.Debug("after split: " + item.Data["cash"] + ", " + newItem.Data["cash"]);

                    Logger.Debug("before: " + item.ToJson(Newtonsoft.Json.Formatting.Indented) + "\n" + newItem.ToJson(Newtonsoft.Json.Formatting.Indented));

                    storageData = GetLocalStorage(client);
                    if (storageData == null) return;

                    storageData.Items.Add(newItem);
                    SetItemOnEmptySlot(client, storageData, newItem);
                    break;
            }
        }

        internal void AddItem(Client client, StorageItemData newItem, StorageData storageData, bool createInNewSlot = false)
        {
            var info = GetItemInfo(newItem.Name);

            if (info == null)
            {
                Logger.Error("This item template doesn't exists");
                return;
            }

            newItem.Count = (newItem.Count > 0 ? newItem.Count : newItem.Count = 1);
            newItem.Data ??= new Dictionary<string, object>();

            var weight = info.Weight * newItem.Count;

            if (info.DefaultData != null)
            {
                foreach (var d in info.DefaultData)
                {
                    if (!newItem.Data.ContainsKey(d.Key))
                    {
                        newItem.Data.Add(d.Key, d.Value);
                    }
                }
            }

            // Temporaire
            if (newItem.Name == "money")
            {
                newItem.Data["cash"] = 500m;
            }

            Logger.Debug("Add item: " + info.Name + ", " + info.CanBeStacked);

            var availableSlot = -1;

            if (HasFreeSpaceForWeight(weight, storageData))
            {
                if (info.CanBeStacked)
                {
                    if (IsSlotExistsWithItemName(client, newItem.Name))
                    {
                        var slot = GetItemByName(newItem.Name, storageData);
                        availableSlot = slot.SlotId;

                        // Besoin d'assigner a newItem le SlotId existant
                        newItem.SlotId = slot.SlotId;
                    }
                    else
                    {
                        availableSlot = GetAvailableSlot(client);
                    }
                }
                else
                {
                    availableSlot = GetAvailableSlot(client);
                }

                if (availableSlot != -1)
                {
                    if (IsSlotAvailable(availableSlot, storageData))
                    {
                        // Créer un nouvelle item dans un slot disponible
                        newItem.SlotId = availableSlot;
                        storageData.Items.Add(newItem);

                        object itemStackValue = null;

                        if(info.OnStacking != null)
                        {
                            newItem.Count = 1;
                        }

                        if (info.CanBeStacked && info.OnRenderStacking != null)
                        {
                            itemStackValue = info.OnRenderStacking.Invoke(newItem);
                        }

                        SetItemOnEmptySlot(client, storageData, newItem);
                    }
                    else
                    {
                        if (info.CanBeStacked)
                        {
                            // Modifie un item dans un slot existant
                            // Modifie la quantité de l'item sur un slot existant

                            if (info.OnStacking != null)
                            {
                                newItem.Count = 1;

                                // Appel une action définis
                                Logger.Debug("Stack on slot: " + availableSlot);

                                var targetItem = GetItemOnSlot(availableSlot, storageData);
                                info.OnStacking.Invoke(newItem, targetItem);
                                var itemIndex = storageData.Items.FindIndex(x => x.SlotId == targetItem.SlotId);

                                storageData.Items.RemoveAt(itemIndex);
                                storageData.Items.Add(targetItem);

                                Logger.Debug("Stack result: " + newItem.Data["cash"] + ", " + targetItem.Data["cash"]);

                                StackItemOnSlot(client, storageData, targetItem);
                            }
                            else
                            {
                                if (!createInNewSlot)
                                {
                                    // Appel l'action par defaut
                                    var itemInstance = storageData.Items.Find(x => x.SlotId == newItem.SlotId);
                                    itemInstance.Count += newItem.Count;

                                    Logger.Debug("Slot count: " + newItem.Name + ", " + newItem.SlotId + ", " + itemInstance.SlotId);

                                    StackItemOnSlot(client, storageData, itemInstance);
                                }
                            }
                        }
                        else
                        {
                            // Créer un nouvelle item dans un slot disponible
                            newItem.SlotId = availableSlot;
                            storageData.Items.Add(newItem);

                            SetItemOnEmptySlot(client, storageData, newItem);
                        }
                    }
                }
                else
                {
                    Logger.Error($"No slot available for: {storageData.Type} -> {storageData.StorageId}");
                }
            }
            else
            {
                Logger.Debug($"[InventoryService] Unable to add item because you have not enought place.");
            }
        }

        private void StackItemOnSlot(Client client, StorageData storageData, StorageItemData itemResult)
        {
            var info = GetItemInfo(itemResult.Name);

            object itemStackValue = null;

            if (info.CanBeStacked && info.OnRenderStacking != null)
            {
                itemStackValue = info.OnRenderStacking.Invoke(itemResult);
            }
            else
            {
                itemStackValue = itemResult.Count;
            }

            var type = GetStorageTypeString(storageData.Type);

            _uiService.SendMessage(client, "storage", "stackItemOnSlot", new
            {
                slotId = itemResult.SlotId,
                count = itemStackValue,
                img = info.Img,

                type
            });
        }

        internal void SetItemOnSlot(Client client, StorageData storageData, int currentSlotId, int targetSlotId)
        {
            var item = GetItemOnSlot(currentSlotId, storageData);
            if (item == null) return;

            var info = GetItemInfo(item.Name);

            var targetItem = GetItemOnSlot(targetSlotId, storageData);
            var haveTarget = targetItem is not null;

            Logger.Debug("current: " + currentSlotId + ", " + targetSlotId);

            var type = GetStorageTypeString(storageData.Type);

            // La cible peu soit être un slot d'item ou un slot vide
            if (haveTarget)
            {
                var targetInfo = GetItemInfo(targetItem.Name);

                if (info.Name != targetInfo.Name)
                {
                    // Les items n'ont pas le même nom
                    Logger.Debug("Alternate items");

                    // On alterne le slotId des cibles pour inverser leur position dans l'interface
                    item.SlotId = targetSlotId;
                    targetItem.SlotId = currentSlotId;

                    // Alterne la position de deux slot, ItemA -> ItemB, ItemB -> ItemA
                    // Si la propriété "CanBeStacked" à la valeur true, les items ne doivent pas être alterner mais "additionner"
                    _uiService.SendMessage(client, "storage", "setItemOnSlot", new
                    {
                        // Base Slot
                        slotId = item.SlotId,
                        count = (info.CanBeStacked && info.OnRenderStacking != null) ? info.OnRenderStacking.Invoke(item) : item.Count,
                        img = info.Img,

                        // Target Slot
                        targetSlotId = targetItem.SlotId,
                        targetCount = (targetInfo.CanBeStacked && targetInfo.OnRenderStacking != null) ? targetInfo.OnRenderStacking.Invoke(targetItem) : targetItem.Count,
                        targetImg = targetInfo.Img,
                        contextItems = GetItemContextMenu(targetItem.Name),
                        
                        type
                    });
                }
                else
                {
                    // Les deux items ont le même nom
                    Logger.Debug("Combine items");
                    StackCombineItem(client, storageData, item, targetItem);
                }
            }
            else
            {
                Logger.Debug("Move slots");

                item.SlotId = targetSlotId;

                // Déplace l'item vers une case vide, Item -> Slot vide
                _uiService.SendMessage(client, "storage", "moveItemOnEmptySlot", new
                {
                    // Base Slot
                    slotId = currentSlotId,

                    // Target Slot
                    targetSlotId = targetSlotId,
                    targetCount = (info.CanBeStacked && info.OnRenderStacking != null) ? info.OnRenderStacking.Invoke(item) : item.Count,
                    targetImg = info.Img,
                    contextItems = GetItemContextMenu(item.Name),

                    type
                });
            }

            switch (storageData.Type)
            {
                case StorageDataType.PlayerInventory:
                    
                    break;
                case StorageDataType.VehicleInventory:
                case StorageDataType.Chest:
                    break;
            }
        }

        private void UpdateSlotRender(Client client, StorageItemData itemData, StorageItemInfo itemInfo, StorageData storageData)
        {
            var type = GetStorageTypeString(storageData.Type);

            _uiService.SendMessage(client, "storage", "updateSlotRender", new
            {
                slotId = itemData.SlotId,
                count = (itemInfo.CanBeStacked && itemInfo.OnRenderStacking != null) ? itemInfo.OnRenderStacking.Invoke(itemData) : itemData.Count,
                img = itemInfo.Img,

                type
            });
        }

        private void StackCombineItem(Client client, StorageData storageData, StorageItemData source, StorageItemData destination)
        {
            var info = GetItemInfo(source.Name);
            var targetInfo = GetItemInfo(destination.Name);

            if (targetInfo.CanBeStacked)
            {
                if (targetInfo.OnStackCombine != null && targetInfo.OnRenderStacking != null)
                {
                    // Action définis ex:(money)
                    Logger.Debug("before combine: " + destination.ToJson());
                    targetInfo.OnStackCombine.Invoke(source, destination);
                    Logger.Debug("after combine: " + destination.ToJson());
                }
                else
                {
                    // Action par defaut ex:(apple)
                    Logger.Debug("before combine: " + destination.ToJson());
                    destination.Count += source.Count;
                    Logger.Debug("after combine: " + destination.ToJson());
                }

                UpdateSlotRender(client, destination, targetInfo, storageData);
                RemoveItemOnSlot(client, storageData, source.SlotId);
            }
        }

        internal void SetItemCountOnSlot(Client client, StorageData storageData, int slotId, int itemCount)
        {
            var item = GetItemOnSlot(slotId, storageData);
            if (item == null) return;

            item.Count = itemCount;
        }

        internal void RemoveItemOnSlot(Client client, StorageData storageData, int slotId)
        {
            var type = GetStorageTypeString(storageData.Type);

            var itemIndex = storageData.Items.FindIndex(x => x.SlotId == slotId);
            if (itemIndex == -1) return;

            storageData.Items.RemoveAt(itemIndex);

            _uiService.SendMessage(client, "storage", "removeItemOnSlot", new
            {
                slotId,

                type
            });
        }

        private void SetItemOnEmptySlot(Client client, StorageData storageData, StorageItemData storageItemData)
        {
            var info = GetItemInfo(storageItemData.Name);
            var type = GetStorageTypeString(storageData.Type);

            _uiService.SendMessage(client, "storage", "setItemOnEmptySlot", new
            {
                slotId = storageItemData.SlotId,
                count = (info.CanBeStacked && info.OnRenderStacking != null) ? info.OnRenderStacking.Invoke(storageItemData) : storageItemData.Count,
                //count = storageItemData.Count,
                img = info.Img,
                contextItems = GetItemContextMenu(storageItemData.Name),

                type
            });
        }

        private List<object> GetItemContextMenu(string itemName)
        {
            var contextMenu = new List<object>();
            var info = GetItemInfo(itemName);

            if (info.ContextMenu != null)
            {
                foreach (var contextItem in info.ContextMenu.Items)
                {
                    contextMenu.Add(new
                    {
                        text = contextItem.Text,
                        emoji = contextItem.Emoji,
                        eventName = contextItem.EventName
                    });
                }
            }

            return contextMenu;
        }

        internal async Task OnStorageContextMenu(Client client, string itemName, int slotId, string eventName, StorageData storageData)
        {
            var info = GetItemInfo(itemName);

            var context = info.ContextMenu.GetContext(eventName);
            if (context == null) return;

            var item = GetItemOnSlot(slotId, storageData);
            if (item == null) return;

            var raycast = await _worldService.GetEntityFrontOfPlayer(client, 6f);

            context.Action.Invoke(client, storageData, item, raycast);
        }

        public async Task<bool> Create(StorageData data) => await _repository.AddAsync(data);
        public async Task<List<StorageData>> GetAll() => await _repository.GetAllAsync();
        public async Task<StorageData> Get(string storageId) => await _repository.GetAsync(x => x.StorageId == storageId);
        public async Task<bool> Update(StorageData data) => await _repository.ReplaceOneAsync(x => x.Id, data.Id, data);
        public async Task<bool> Delete(StorageData data) => await _repository.DeleteOneAsync(x => x.Id == data.Id);
        public async Task<bool> Delete(string storageId) => await _repository.DeleteOneAsync(x => x.StorageId == storageId);
        public async Task<bool> Exists(StorageData data) => await _repository.ExistsAsync(x => x.Id == data.Id);
        public async Task<bool> Exists(string storageId) => await _repository.ExistsAsync(x => x.StorageId == storageId);
    }
}