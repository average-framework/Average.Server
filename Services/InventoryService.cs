using Average.Server.Framework.Diagnostics;
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
        private readonly ClientService _clientService;

        private readonly List<StorageItemInfo> _items = new();
        private readonly Dictionary<string, Dictionary<string, object>> _clients = new();

        public const double DefaultMaxInventoryWeight = 20.0;
        public const double DefaultMaxChestWeight = 100.0;

        public const int InventorySlotCount = 20;
        public const int VehicleSlotCount = 8;
        public const int ChestSlotCount = 8;
        public const int BankSlotCount = 8;
        public const int TradeSlotCount = 16;

        private const bool SaveOnChanged = true;

        public InventoryService(ClientService clientService, WorldService worldService, RpcService rpcService, InventoryRepository repository, UIService uiService, InputService inputService)
        {
            _worldService = worldService;
            _rpcService = rpcService;
            _repository = repository;
            _uiService = uiService;
            _inputService = inputService;
            _clientService = clientService;

            // Events
            _worldService.TimeChanged += OnTimeChanged;

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
                OpenInventory(client);
                Logger.Debug($"Client {client.Name} open inventory");
            }));

            Logger.Write("InventoryService", "Initialized successfully");
        }

        private void OnTimeChanged(object sender, WorldService.WorldTimeEventArgs e)
        {
            try
            {
                for (int i = 0; i < _clientService.ClientCount; i++)
                {
                    var client = _clientService[i];
                    SetTime(client, e.Time);
                }
            }
            catch
            {
                Logger.Error("[InventoryService] Unable to set time for clients.");
            }
        }

        internal async void OnClientInitialized(Client client, string characterId)
        {
            if (!_clients.ContainsKey(client.License))
            {
                var storage = await Get(characterId);

                if (storage == null) return;

                var dict = new Dictionary<string, object>();

                dict.Add("Storage", storage ?? new StorageData
                {
                    //StorageId = client.License,
                    StorageId = characterId,
                    MaxWeight = DefaultMaxInventoryWeight,
                    Type = StorageDataType.Player
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

        internal void InitSlots(Client client, StorageDataType storageType, int slotCount)
        {
            var type = GetStorageTypeString(storageType);

            _uiService.SendMessage(client, "storage", "init", new
            {
                slotCount,

                type
            });
        }

        internal async Task<bool> SaveInventory(Client client)
        {
            var storage = GetLocalStorage(client);
            if (storage == null) return false;

            return await Update(storage);
        }

        internal void LoadInventory(Client client, StorageData storageData)
        {
            foreach (var item in storageData.Items)
            {
                SetItemOnEmptySlot(client, storageData, item);
            }
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

        internal void OpenInventory(Client client)
        {
            if (!GetData<bool>(client, "IsInventoryOpen"))
            {
                SetData(client, "IsInventoryOpen", true);

                _uiService.SendMessage(client, "storage", "inventory_open");
            }
        }

        internal void OpenChest(Client client, StorageData storageData)
        {
            if (!GetData<bool>(client, "IsChestOpen"))
            {
                SetData(client, "IsChestOpen", true);
                SetData(client, "ChestData", storageData);

                _uiService.SendMessage(client, "storage", "chest_open");
            }
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

        internal void CloseInventory(Client client)
        {
            if (GetData<bool>(client, "IsInventoryOpen"))
            {
                SetData(client, "IsInventoryOpen", false);

                _uiService.SendMessage(client, "storage", "inventory_close");
            }
        }

        internal void CloseChest(Client client)
        {
            if (GetData<bool>(client, "IsChestOpen"))
            {
                SetData(client, "IsChestOpen", false);
                SetData(client, "CurrentChest", null);

                _uiService.SendMessage(client, "storage", "chest_close");
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

        internal void SetInventoryWeight(Client client, StorageData storageData)
        {
            var type = GetStorageTypeString(storageData.Type);

            _uiService.SendMessage(client, "storage", "setWeight", new
            {
                weight = CalculateWeight(storageData).ToString("0.00"),
                maxWeight = storageData.MaxWeight.ToString("0.00"),

                type
            });
        }

        internal void SetTime(Client client, TimeSpan time)
        {
            _uiService.SendMessage(client, "storage", "setTime", new
            {
                time = string.Format($"{(time.Hours < 10 ? "0" + time.Hours : time.Hours)}h{(time.Minutes < 10 ? "0" + time.Minutes : time.Minutes)}", time)
            });
        }

        internal void SetTemperature(Client client, int temperature)
        {
            _uiService.SendMessage(client, "storage", "setTemperature", new
            {
                temperature = temperature + "°C"
            });
        }

        internal double CalculateWeight(StorageData storageData)
        {
            var weight = 0d;
            storageData.Items.ForEach(x => weight += GetItemInfo(x.Name).Weight * x.Count);
            return weight;
        }

        internal bool HasFreeSpaceForWeight(StorageItemData itemData, StorageData storageData)
        {
            var info = GetItemInfo(itemData.Name);
            var totalNeededWeight = CalculateWeight(storageData) + itemData.Count * info.Weight;

            Logger.Error("Weight: " + totalNeededWeight + ", " + storageData.MaxWeight);
            Logger.Error("Has Free Space: " + (totalNeededWeight <= storageData.MaxWeight));

            if (totalNeededWeight > storageData.MaxWeight)
            {
                if(info.CanBeStacked && info.OnStacking != null)
                {
                    return true;
                }
            }

            return CalculateWeight(storageData) + totalNeededWeight <= storageData.MaxWeight;
        }

        //internal bool HasFreeSpaceForWeight(double itemWeight, StorageData storageData)
        //{
        //    Logger.Error("Weight: " + (CalculateWeight(storageData) + itemWeight) + ", " + storageData.MaxWeight);
        //    Logger.Error("Has Free Space: " + ((CalculateWeight(storageData) + itemWeight) <= storageData.MaxWeight));

        //    return CalculateWeight(storageData) + itemWeight <= storageData.MaxWeight;
        //}

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

        internal bool IsSlotAvailable(Client client, int slotIndex)
        {
            var storage = GetLocalStorage(client);
            if (storage == null) return false;

            return storage.Items.Exists(x => x.SlotId == slotIndex);
        }

        internal bool IsSlotExistsWithItemName(Client client, StorageData storageData, string itemName)
        {
            //var storage = GetLocalStorage(client);
            if (storageData == null) return false;

            return storageData.Items.Exists(x => x.Name == itemName);
        }

        internal int GetAvailableSlot(StorageData storageData)
        {
            if (storageData == null) return -1;

            var slotCount = 0;

            switch (storageData.Type)
            {
                case StorageDataType.Player:
                    slotCount = InventorySlotCount;
                    break;
                case StorageDataType.Vehicle:
                    slotCount = VehicleSlotCount;
                    break;
                case StorageDataType.Chest:
                    slotCount = ChestSlotCount;
                    break;
                case StorageDataType.Bank:
                    slotCount = BankSlotCount;
                    break;
                case StorageDataType.Trade:
                    slotCount = TradeSlotCount;
                    break;
            }


            for (int i = 0; i < slotCount; i++)
            {
                if (!storageData.Items.Exists(x => x.SlotId == i))
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
                case StorageDataType.Player:
                    type = "inv";
                    break;
                case StorageDataType.Vehicle:
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

            switch (value)
            {
                case int convertedValue:
                    if (item.Count == (int)minValue) return;

                    var valResult = 0;

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
                    }

                    var newSlotId = GetAvailableSlot(storageData);
                    var newItem = new StorageItemData(item.Name, (int)maxValue - valResult);
                    newItem.SlotId = newSlotId;

                    Logger.Error("New slot id after split: " + slotId + ", " + newSlotId + ", " + storageData.Id + ", " + storageData.Type);

                    var newDictionary = item.Data.ToDictionary(entry => entry.Key, entry => entry.Value);
                    newItem.Data = newDictionary;

                    if (storageData == null) return;

                    storageData.Items.Add(newItem);
                    SetItemOnEmptySlot(client, storageData, newItem);

                    // Appel l'action par defaut
                    // Met à jour l'affichage du premier item
                    UpdateSlotRender(client, item, info, storageData);

                    if (SaveOnChanged)
                    {
                        Update(storageData);
                    }
                    break;
                case decimal convertedValue:
                    var canSplit = info.SplitCondition != null && info.SplitCondition.Invoke(item);
                    if (!canSplit) return;

                    var valDecResult = 0m;

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

                    if (info.OnSplit != null)
                    {
                        info.OnSplit.Invoke(item, valDecResult, StorageItemInfo.SplitType.BaseItem);
                    }

                    newSlotId = GetAvailableSlot(storageData);
                    newItem = new StorageItemData(item.Name, 1);
                    newItem.SlotId = newSlotId;

                    // Besoin de copier les données, sinon les données du base item et du target item sont encore lier
                    newDictionary = item.Data.ToDictionary(entry => entry.Key, entry => entry.Value);

                    newItem.Data = newDictionary;
                    info.OnSplit.Invoke(newItem, valDecResult, StorageItemInfo.SplitType.TargetItem);

                    if (storageData == null) return;

                    storageData.Items.Add(newItem);
                    SetItemOnEmptySlot(client, storageData, newItem);

                    // Appel l'action par defaut
                    // Met à jour l'affichage du premier item
                    UpdateSlotRender(client, item, info, storageData);

                    if (SaveOnChanged)
                    {
                        Update(storageData);
                    }
                    break;
            }
        }

        internal void AddItem(Client client, StorageItemData newItem, StorageData storageData, bool createInNewSlot = false, int createInSlotId = 0)
        {
            var info = GetItemInfo(newItem.Name);

            if (info == null)
            {
                Logger.Error("This item template doesn't exists");
                return;
            }

            newItem.Count = (newItem.Count > 0 ? newItem.Count : newItem.Count = 1);
            newItem.Data ??= new Dictionary<string, object>();

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

            var availableSlot = -1;

            if (HasFreeSpaceForWeight(newItem, storageData))
            {
                if (info.CanBeStacked)
                {
                    if (IsSlotExistsWithItemName(client, storageData, newItem.Name))
                    {
                        var slot = GetItemByName(newItem.Name, storageData);
                        if (slot == null) return;

                        availableSlot = slot.SlotId;

                        // Besoin d'assigner a newItem le SlotId existant
                        newItem.SlotId = slot.SlotId;
                    }
                    else
                    {
                        availableSlot = GetAvailableSlot(storageData);
                    }
                }
                else
                {
                    availableSlot = GetAvailableSlot(storageData);
                }

                if (availableSlot != -1 || createInNewSlot)
                {
                    Logger.Debug("available ? " + availableSlot + ", " + storageData.StorageId);

                    if (createInNewSlot)
                    {
                        availableSlot = createInSlotId;
                    }

                    if (IsSlotAvailable(availableSlot, storageData))
                    {
                        if (!createInNewSlot)
                        {
                            Logger.Debug("Slot -> 0");

                            // Créer un nouvelle item dans un slot disponible
                            newItem.SlotId = availableSlot;
                            storageData.Items.Add(newItem);

                            if (info.OnStacking != null)
                            {
                                newItem.Count = 1;
                            }

                            SetItemOnEmptySlot(client, storageData, newItem);
                        }
                        else
                        {
                            Logger.Debug("Slot -> 1");

                            newItem.SlotId = createInSlotId;
                            storageData.Items.Add(newItem);

                            if (info.OnStacking != null)
                            {
                                newItem.Count = 1;
                            }

                            SetItemOnEmptySlot(client, storageData, newItem);
                        }

                        if (SaveOnChanged)
                        {
                            Update(storageData);
                        }
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
                                var targetItem = GetItemOnSlot(availableSlot, storageData);
                                //info.OnStacking.Invoke(newItem, targetItem);
                                var itemIndex = storageData.Items.FindIndex(x => x.SlotId == targetItem.SlotId);

                                storageData.Items.RemoveAt(itemIndex);
                                storageData.Items.Add(targetItem);

                                StackItemOnSlot(client, storageData, newItem, targetItem);

                                if (SaveOnChanged)
                                {
                                    Update(storageData);
                                }
                            }
                            else
                            {
                                if (!createInNewSlot)
                                {
                                    Logger.Debug("Slot -> 2");

                                    // Appel l'action par defaut
                                    var itemInstance = storageData.Items.Find(x => x.SlotId == newItem.SlotId);
                                    //itemInstance.Count += newItem.Count;

                                    StackItemOnSlot(client, storageData, newItem, itemInstance);

                                    if (SaveOnChanged)
                                    {
                                        Update(storageData);
                                    }
                                }
                                //else
                                //{
                                //    Logger.Debug("Slot -> 3");
                                //}
                            }
                        }
                        else
                        {
                            // Créer un nouvelle item dans un slot disponible
                            newItem.SlotId = availableSlot;
                            storageData.Items.Add(newItem);
                            
                            SetItemOnEmptySlot(client, storageData, newItem);

                            if (SaveOnChanged)
                            {
                                Update(storageData);
                            }
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

        internal void StackItemOnSlot(Client client, StorageData storageData, StorageItemData source, StorageItemData destination)
        {
            var info = GetItemInfo(destination.Name);

            object itemStackValue = null;

            if (info.CanBeStacked)
            {
                if (info.OnStacking != null)
                {
                    // Modifie le nombre d'item sur l'instance de l'item
                    info.OnStacking.Invoke(source, destination);
                }
                else
                {
                    // Modifie le nombre d'item sur l'instance de l'item
                    destination.Count += source.Count;
                }
            }
            else
            {
                // Modifie le nombre d'item sur l'instance de l'item
                destination.Count += source.Count;
            }

            if (info.CanBeStacked && info.OnRenderStacking != null)
            {
                itemStackValue = info.OnRenderStacking.Invoke(destination);
            }
            else
            {
                itemStackValue = destination.Count;
            }

            var type = GetStorageTypeString(storageData.Type);

            _uiService.SendMessage(client, "storage", "stackItemOnSlot", new
            {
                slotId = destination.SlotId,
                count = itemStackValue,
                img = info.Img,

                type
            });

            SetInventoryWeight(client, storageData);
        }

        internal void SetItemOnSlot(Client client, StorageData storageData, int currentSlotId, int targetSlotId)
        {
            var item = GetItemOnSlot(currentSlotId, storageData);
            if (item == null) return;

            var info = GetItemInfo(item.Name);

            var targetItem = GetItemOnSlot(targetSlotId, storageData);
            var haveTarget = targetItem is not null;

            var type = GetStorageTypeString(storageData.Type);

            // La cible peu soit être un slot d'item ou un slot vide
            if (haveTarget)
            {
                var targetInfo = GetItemInfo(targetItem.Name);

                if (info.Name != targetInfo.Name)
                {
                    // Les items n'ont pas le même nom
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
                    StackCombineItem(client, storageData, item, targetItem);
                }

                if (SaveOnChanged)
                {
                    Update(storageData);
                }
            }
            else
            {
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

                if (SaveOnChanged)
                {
                    Update(storageData);
                }
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
                    targetInfo.OnStackCombine.Invoke(source, destination);
                }
                else
                {
                    // Action par defaut ex:(apple)
                    destination.Count += source.Count;
                }

                UpdateSlotRender(client, destination, targetInfo, storageData);
                RemoveItemOnSlot(client, storageData, source.SlotId);

                SetInventoryWeight(client, storageData);
            }
        }

        internal void SetItemCountOnSlot(Client client, StorageData storageData, int slotId, int itemCount)
        {
            var item = GetItemOnSlot(slotId, storageData);
            if (item == null) return;

            item.Count = itemCount;

            var info = GetItemInfo(item.Name);
            UpdateSlotRender(client, item, info, storageData);

            SetInventoryWeight(client, storageData);
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

            SetInventoryWeight(client, storageData);
        }

        internal void SetItemOnEmptySlot(Client client, StorageData storageData, StorageItemData storageItemData)
        {
            var info = GetItemInfo(storageItemData.Name);
            var type = GetStorageTypeString(storageData.Type);

            _uiService.SendMessage(client, "storage", "setItemOnEmptySlot", new
            {
                slotId = storageItemData.SlotId,
                count = (info.CanBeStacked && info.OnRenderStacking != null) ? info.OnRenderStacking.Invoke(storageItemData) : storageItemData.Count,
                img = info.Img,
                contextItems = GetItemContextMenu(storageItemData.Name),

                type
            });

            SetInventoryWeight(client, storageData);
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