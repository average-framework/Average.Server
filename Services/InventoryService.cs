using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Models;
using Average.Server.Repositories;
using Average.Shared.DataModels;
using Average.Shared.Enums;
using CitizenFX.Core;
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
        private readonly PlayerList _players;
        private readonly WorldService _worldService;

        private readonly List<StorageItemInfo> _items = new();

        private readonly Dictionary<string, Dictionary<string, object>> _clients = new();

        public const double DefaultMaxInventoryWeight = 20.0;
        public const double DefaultMaxChestWeight = 100.0;

        private const int SlotCount = 20;

        public InventoryService(WorldService worldService, RpcService rpcService, InventoryRepository repository, UIService uiService, InputService inputService, PlayerList players)
        {
            _worldService = worldService;
            _rpcService = rpcService;
            _repository = repository;
            _uiService = uiService;
            _inputService = inputService;
            _players = players;

            //_items = Configuration.Parse<List<StorageItemInfo>>("configs/storage_items.json");

            RegisterItem(new StorageItemInfo
            {
                Name = "money",
                Img = "money_moneystack",
                Title = "Du fric",
                Description = "Un petit peu de flouz",
                Weight = 1.0,
                CanBeStacked = true,
                OnStacking = (lastItem, targetItem) =>
                {
                    var cash = decimal.Parse(lastItem.Data["cash"].ToString()) + decimal.Parse(targetItem.Data["cash"].ToString());
                    targetItem.Data["cash"] = cash;
                    return targetItem;
                },
                OnRenderStack = (item) =>
                {
                    return item.Data["cash"];
                },
                ContextMenu = new StorageContextMenu(new StorageContextItem
                {
                    EventName = "drop",
                    Text = "Jeter par la fenêtre",
                    Name = "drop",
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
                dict.Add("IntegerInputValue", 0);
                dict.Add("DecimalInputValue", 0m);

                _clients.Add(client.License, dict);
            }

            //_uiService.LoadFrame(client, "storage");
            //_uiService.SetZIndex(client, "storage", 90000);
        }

        internal void OnClientWindowInitialized(Client client)
        {
            _uiService.LoadFrame(client, "storage");
            _uiService.SetZIndex(client, "storage", 90000);
        }

        internal void MoveItemToSlot(StorageItemData item, StorageData storageData)
        {

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

        //private IEnumerable<Player> GetPlayersAroundPlayer(Client client)
        //{
        //    var position = client.Player.Character.Position;
        //    var distance = 6f;

        //    return _players.ToList().Where(x => x.Character.Position.DistanceToSquared(position) <= distance);
        //}

        //private void GiveItemToPlayer(Client client, Client target, StorageItemData itemData, int itemCount)
        //{
        //    var storage = GetLocalStorage(client);
        //    var targetStorage = GetLocalStorage(target);

        //    var item = storage?.Items.Find(x => x.Name == itemData.Name);
        //    if (item == null) return;

        //    var info = GetItemInfo(item.Name);
        //    if (info == null) return;

        //    if (itemCount <= item.Count)
        //    {
        //        if (HasFreeSpace(targetStorage))
        //        {
        //            if (info.RemoveOnGive)
        //            {
        //                RemoveItemOnSlot(client, item.SlotId, itemCount, storage);
        //                UpdateRender(client, storage);
        //            }

        //            var newItem = itemData;
        //            newItem.Count = itemCount;

        //            AddItem(target, newItem, targetStorage);
        //            UpdateRender(target, targetStorage);
        //        }
        //    }
        //}

        //private void RemoveItemToPlayerOnSlot(Client client, Client target, int slotId, int itemCount)
        //{
        //    var targetStorage = GetLocalStorage(target);
        //    var item = targetStorage?.Items.Find(x => x.SlotId == slotId);

        //    if (item == null) return;

        //    RemoveItemOnSlot(target, slotId, itemCount, targetStorage);
        //    UpdateRender(target, targetStorage);

        //    Logger.Debug($"Client: {client.Name} remove item to target: {target.Name}");
        //}

        //private void MoveItem(Client client, StorageData from, StorageData to, int slotId, int itemCount)
        //{
        //    var item = from?.Items.Find(x => x.SlotId == slotId);
        //    var info = GetItemInfo(item.Name);

        //    if (itemCount <= item.Count)
        //    {
        //        if (HasFreeSpaceForWeight(info.Weight * itemCount, to))
        //        {
        //            AddItem(client, item, to);
        //            RemoveItemOnSlot(client, item.SlotId, itemCount, from);
        //            UpdateRender(client, from);
        //            UpdateRender(client, to);
        //        }
        //    }
        //}

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

                var storage = GetLocalStorage(client) ?? await LoadStorage(client, client.License);

                Logger.Debug("Open inventory: " + storage.ToJson());

                //UpdateRender(client, storage);

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

        //private void StackSlots(int slotId, StorageItemData newItem, StorageData storageData)
        //{
        //    var info = GetItemInfo(newItem.Name);
        //    var targetItem = GetItemOnSlot(slotId, storageData);
        //    var itemResult = info.OnStacking.Invoke(newItem, targetItem);
        //    var itemIndex = storageData.Items.FindIndex(x => x.SlotId == targetItem.SlotId);

        //    storageData.Items.RemoveAt(itemIndex);
        //    storageData.Items.Add(itemResult);
        //}

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

            // Test temporaire
            newItem.Data.Add("cash", 15m);

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

                //SetItemOnEmptySlot(client, storageData, newItem);
            }
            else
            {
                Logger.Debug($"[InventoryService] Unable to add item because you have not enought place.");

                // Ancien fix, encore besoin ???
                // newItem.Count = storageData.Items.Find(x => x.Name == newItem.Name).Count;
            }
        }

        //internal void RemoveItemOnSlot(Client client, int slotId, int itemCount, StorageData storageData)
        //{
        //    var item = storageData.Items.Find(x => x.SlotId == slotId);
        //    if (item == null) return;

        //    if (IsSlotAvailable(slotId, storageData))
        //    {
        //        if (item.Count - itemCount >= 0)
        //        {
        //            item.Count -= itemCount;

        //            if (item.Count == 0)
        //            {
        //                storageData.Items.RemoveAll(x => x.Name == item.Name);
        //            }

        //            UpdateRender(client, storageData);
        //        }
        //    }
        //    else
        //    {
        //        Logger.Debug($"[InventoryService] Unable to remove item because id: {slotId} does not exists.");
        //    }
        //}

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
                            targetImg = targetInfo.Img
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
                            targetImg = info.Img
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
                        img = info.Img
                    });
                    break;
                case StorageDataType.VehicleInventory:
                case StorageDataType.Chest:
                    break;
            }
        }

        //private void UpdateRender(Client client, StorageData storageData)
        //{
        //    var items = new List<object>();

        //    foreach (var item in storageData.Items)
        //    {
        //        var contextMenu = new List<object>();

        //        var info = GetItemInfo(item.Name);
        //        if (info == null) continue;

        //        info.OnUpdateRender?.Invoke();

        //        if (info.ContextMenu != null)
        //        {
        //            foreach (var contextItem in info.ContextMenu.Items)
        //            {
        //                contextItem.Id = SharedAPI.RandomString();

        //                contextMenu.Add(new
        //                {
        //                    name = contextItem.Name,
        //                    id = contextItem.Id,
        //                    text = contextItem.Text,
        //                    emoji = contextItem.Emoji,
        //                    eventName = contextItem.EventName
        //                });
        //            }
        //        }

        //        items.Add(new
        //        {
        //            slotId = item.SlotId,
        //            title = info.Title,
        //            description = info.Description,
        //            img = info.Img,
        //            count = item.Count,
        //            menu = contextMenu
        //        });
        //    }

        //    switch (storageData.Type)
        //    {
        //        case StorageDataType.PlayerInventory:
        //            UpdateInventoryRender(client, storageData, items);
        //            break;
        //        case StorageDataType.VehicleInventory:
        //        case StorageDataType.Chest:
        //            UpdateChestRender(client, storageData, items);
        //            break;
        //    }
        //}

        //private void UpdateInventoryRender(Client client, StorageData storageData, List<object> items)
        //{
        //    _uiService.SendMessage(client, "storage", "render_inventory", new
        //    {
        //        weight = CalculateWeight(storageData).ToString("0.00"),
        //        maxWeight = storageData.MaxWeight,
        //        items
        //    });
        //}

        //private void UpdateChestRender(Client client, StorageData storageData, List<object> items)
        //{
        //    _uiService.SendMessage(client, "storage", "render_chest", new
        //    {
        //        weight = CalculateWeight(storageData).ToString("0.00"),
        //        maxWeight = storageData.MaxWeight,
        //        items
        //    });
        //}

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

        //internal void OnStorageInputCount(Client client, StorageData storageData, string value)
        //{
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        decimal.TryParse(value, out decimal decInput);
        //        int.TryParse(value, out int intInput);

        //        if (intInput < 1)
        //        {
        //            intInput = 1;
        //        }

        //        if (decInput <= 0m)
        //        {
        //            decInput = 0m;
        //        }

        //        SetData(client, "IntegerInputValue", intInput);
        //        SetData(client, "DecimalInputValue", decInput);

        //        //UpdateRender(client, storageData);
        //    }
        //}

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