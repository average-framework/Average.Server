using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Models;
using Average.Server.Repositories;
using Average.Shared;
using Average.Shared.DataModels;
using Average.Shared.Enums;
using System.Collections.Generic;
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

            RegisterItem(new StorageItemInfo
            {
                Name = "money",
                Img = "money_moneystack",
                Title = "Du fric",
                Description = "Un petit peu de flouz",
                Weight = 1.0,
                CanBeStacked = true,
                //OnStacking = (lastItem, targetItem) =>
                //{
                //    var cash = decimal.Parse(lastItem.Data["cash"].ToString()) + decimal.Parse(targetItem.Data["cash"].ToString());
                //    targetItem.Data["cash"] = cash;
                //    return targetItem;
                //},
                //OnRenderStack = (item) =>
                //{
                //    return item.Data["cash"];
                //},
                ContextMenu = new StorageContextMenu(new StorageContextItem
                {
                    EventName = "drop",
                    Text = "Jeter par la fenêtre",
                    Action = (itemData, raycast) =>
                    {
                        Logger.Debug("item: " + itemData.Name + ", " + raycast.EntityHit);
                    }
                })
            });

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

        internal async void OnClientInitialized(Client client)
        {
            if (!_clients.ContainsKey(client.License))
            {
                var dict = new Dictionary<string, object>();
                var storage = await Get(client.License);

                dict.Add("Storage", storage ?? new StorageData
                {
                    StorageId = client.License,
                    MaxWeight = DefaultMaxInventoryWeight,
                    Type = StorageDataType.PlayerInventory
                });
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

        internal void InitSlots(Client client) => _uiService.SendMessage(client, "storage", "init", new
        {
            slotCount = SlotCount
        });

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

        internal void AddItem(Client client, StorageItemData newItem, StorageData storageData)
        {
            var info = GetItemInfo(newItem.Name);

            if (info == null)
            {
                Logger.Error("This item template doesn't exists");
                return;
            }

            newItem.Count = (newItem.Count > 0 ? newItem.Count : newItem.Count = 1);
            var weight = info.Weight * newItem.Count;

            newItem.Data ??= new Dictionary<string, object>();

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
                        newItem.SlotId = availableSlot;
                        storageData.Items.Add(newItem);

                        SetItemOnEmptySlot(client, storageData, newItem);
                    }
                    else
                    {
                        if (info.CanBeStacked)
                        {
                            if (info.OnStacking != null)
                            {
                                // Appel une action définis
                                Logger.Debug("Stack on slot: " + availableSlot);

                                var targetItem = GetItemOnSlot(availableSlot, storageData);
                                var itemResult = info.OnStacking.Invoke(newItem, targetItem);
                                var itemIndex = storageData.Items.FindIndex(x => x.SlotId == targetItem.SlotId);

                                storageData.Items.RemoveAt(itemIndex);
                                storageData.Items.Add(itemResult);

                                Logger.Debug("Stack result: " + newItem.Data["cash"] + ", " + targetItem.Data["cash"] + ", " + itemResult.Data["cash"]);

                                StackItemOnSlot(client, storageData, itemResult);
                            }
                            else
                            {
                                // Appel l'action par defaut
                                var itemInstance = storageData.Items.Find(x => x.SlotId == newItem.SlotId);
                                itemInstance.Count += newItem.Count;

                                Logger.Debug("Slot count: " + newItem.Name + ", " + newItem.SlotId + ", " + itemInstance.SlotId);

                                StackItemOnSlot(client, storageData, itemInstance);
                            }
                        }
                        else
                        {
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

            if (info.CanBeStacked && info.OnRenderStack != null)
            {
                itemStackValue = info.OnRenderStack.Invoke(itemResult);
            }
            else
            {
                itemStackValue = itemResult.Count;
            }

            switch (storageData.Type)
            {
                case StorageDataType.PlayerInventory:
                    _uiService.SendMessage(client, "storage", "stackItemOnSlot", new
                    {
                        slotId = itemResult.SlotId,
                        count = itemStackValue,
                        img = info.Img
                    });
                    break;
                case StorageDataType.VehicleInventory:
                case StorageDataType.Chest:
                    break;
            }
        }

        internal void SetItemOnSlot(Client client, StorageData storageData, int currentSlotId, int targetSlotId)
        {
            var item = GetItemOnSlot(currentSlotId, storageData);
            var info = GetItemInfo(item.Name);

            var targetItem = GetItemOnSlot(targetSlotId, storageData);
            StorageItemInfo targetInfo = null;

            var targetExists = targetItem is not null;

            // La cible peu soit être un slot d'item ou un slot vide
            if (targetItem is not null)
            {
                targetItem.SlotId = currentSlotId;
                targetInfo = GetItemInfo(targetItem.Name);
            }

            // On alterne le slotId des cibles pour inverser leur position dans l'interface
            item.SlotId = targetSlotId;

            var target1 = GetItemOnSlot(currentSlotId, storageData);
            var item1 = GetItemOnSlot(targetSlotId, storageData);

            Logger.Debug("current: " + currentSlotId + ", " + targetSlotId);
            Logger.Debug("move item: " + item.SlotId + ", " + targetItem?.SlotId + ", " + target1?.SlotId + ", " + item1?.SlotId);

            switch (storageData.Type)
            {
                case StorageDataType.PlayerInventory:
                    if (targetExists)
                    {
                        Logger.Debug("Alternate");

                        // Alterne la position de deux slot, ItemA -> ItemB, ItemB -> ItemA
                        // Si la propriété "CanBeStacked" à la valeur true, les items ne doivent pas être alterner mais "additionner"
                        _uiService.SendMessage(client, "storage", "setItemOnSlot", new
                        {
                            // Base Slot
                            slotId = item.SlotId,
                            count = item.Count,
                            img = info.Img,

                            // Target Slot
                            targetSlotId = targetItem.SlotId,
                            targetCount = targetItem.Count,
                            targetImg = targetInfo.Img,
                            contextItems = GetItemContextMenu(targetItem.Name)
                        });
                    }
                    else
                    {
                        Logger.Debug("Move slots");

                        // Déplace l'item vers une case vide, Item -> Slot vide
                        _uiService.SendMessage(client, "storage", "moveItemOnEmptySlot", new
                        {
                            // Base Slot
                            slotId = currentSlotId,

                            // Target Slot
                            targetSlotId = targetSlotId,
                            targetCount = item.Count,
                            targetImg = info.Img,
                            contextItems = GetItemContextMenu(item.Name)
                        });
                    }
                    break;
                case StorageDataType.VehicleInventory:
                case StorageDataType.Chest:
                    break;
            }
        }

        private void SetItemOnEmptySlot(Client client, StorageData storageData, StorageItemData storageItemData)
        {
            var info = GetItemInfo(storageItemData.Name);
            if (info == null) return;

            switch (storageData.Type)
            {
                case StorageDataType.PlayerInventory:
                    _uiService.SendMessage(client, "storage", "setItemOnEmptySlot", new
                    {
                        slotId = storageItemData.SlotId,
                        count = storageItemData.Count,
                        img = info.Img,
                        contextItems = GetItemContextMenu(storageItemData.Name)
                    });
                    break;
                case StorageDataType.VehicleInventory:
                case StorageDataType.Chest:
                    break;
            }
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
            if (info == null) return;

            var context = info.ContextMenu.GetContext(eventName);
            if (context == null) return;

            var item = GetItemOnSlot(slotId, storageData);
            if (item == null) return;

            var raycast = await _worldService.GetEntityFrontOfPlayer(client, 6f);

            context.Action.Invoke(item, raycast);
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