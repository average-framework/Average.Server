using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Models;
using Average.Server.Repositories;
using Average.Shared.Attributes;
using Average.Shared.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Average.Server.Services.RpcService;

namespace Average.Server.Services
{
    internal class InventoryService : IService
    {
        private readonly InventoryRepository _repository;
        private readonly UIService _uiService;

        private readonly List<StorageItemInfo> _items = new();

        public InventoryService(InventoryRepository repository, UIService uiService)
        {
            _repository = repository;
            _uiService = uiService;

            _items = Configuration.Parse<List<StorageItemInfo>>("configs/storage_items.json");
        }

        [UICallback("inventorytest")]
        private void OnTest(Dictionary<string, object> args, RpcCallback cb)
        {
            Logger.Error("inventorytest triggered: " + string.Join(", ", args));

            cb("enculer", "enculer 2");
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

        internal bool ItemExistsById(string uniqueId, StorageData storageData)
        {
            return storageData.Items.Exists(x => x.UniqueId == uniqueId);
        }

        internal bool ItemExistsByName(string itemName, StorageData storageData)
        {
            return storageData.Items.Exists(x => x.Name == itemName);
        }

        internal StorageItemData GetItemById(string uniqueId, StorageData storageData)
        {
            return storageData.Items.Find(x => x.UniqueId == uniqueId);
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

        internal void AddItem(Client client, StorageItemData newItem, StorageData storageData)
        {
            newItem.Count = (newItem.Count > 0 ? newItem.Count : newItem.Count = 1);

            var info = GetItemInfo(newItem.Name);
            var weight = info.Weight * newItem.Count;

            newItem.Data ??= new Dictionary<string, object>();

            if (!info.Stackable)
            {
                newItem.Count = 1;
            }

            if (HasFreeSpaceForWeight(weight, storageData))
            {
                if (!ItemExistsByName(newItem.Name, storageData))
                {
                    storageData.Items.Add(newItem);
                }
                else
                {
                    if (!info.Stackable)
                    {
                        storageData.Items.Add(newItem);
                    }
                    else
                    {
                        storageData.Items.Find(x => x.Name == newItem.Name).Count += newItem.Count;
                    }
                }

                UpdateRender(client, storageData);
            }
            else
            {
                Logger.Debug($"[InventoryService] Unable to add item because you have not enought place.");

                // Ancien fix, encore besoin ???
                //newItem.Count = storageData.Items.Find(x => x.Name == newItem.Name).Count;
            }
        }

        internal void RemoveItemByName(Client client, StorageItemData itemData, int count, StorageData storageData)
        {
            if (ItemExistsByName(itemData.Name, storageData))
            {
                if (itemData.Count - count >= 0)
                {
                    itemData.Count -= count;

                    if (itemData.Count == 0)
                    {
                        storageData.Items.RemoveAll(x => x.Name == itemData.Name);
                    }

                    UpdateRender(client, storageData);
                }
            }
            else
            {
                Logger.Debug($"[InventoryService] Unable to remove item because does not exists.");
            }
        }

        internal void RemoveItemById(Client client, StorageItemData itemData, int count, StorageData storageData)
        {
            if (ItemExistsById(itemData.UniqueId, storageData))
            {
                if (itemData.Count - count >= 0)
                {
                    itemData.Count -= count;

                    if (itemData.Count == 0)
                    {
                        storageData.Items.RemoveAll(x => x.Name == itemData.Name);
                    }

                    UpdateRender(client, storageData);
                }
            }
            else
            {
                Logger.Debug($"[InventoryService] Unable to remove item because id: {itemData.UniqueId} does not exists.");
            }
        }

        private void UpdateRender(Client client, StorageData storageData)
        {
            var items = new List<object>();

            foreach (var item in storageData.Items)
            {
                var contextMenu = new List<object>();
                var info = GetItemInfo(item.Name);

                info.OnUpdateRender?.Invoke(item);

                if (info.ContextMenu != null)
                {
                    foreach (var contextItem in info.ContextMenu.Items)
                    {
                        contextItem.Id = Shared.SharedAPI.RandomString();

                        contextMenu.Add(new
                        {
                            name = contextItem.Name,
                            id = contextItem.Id,
                            text = contextItem.Text,
                            emoji = contextItem.Emoji,
                            eventName = contextItem.EventName
                        });
                    }
                }

                items.Add(new
                {
                    id = item.UniqueId,
                    text = info.Text,
                    img = info.Img,
                    count = item.Count,
                    menu = contextMenu
                });
            }

            switch (storageData.Type)
            {
                case StorageDataType.PlayerInventory:
                    UpdateInventoryRender(client, storageData, items);
                    break;
                case StorageDataType.VehicleInventory:
                case StorageDataType.Chest:
                    UpdateChestRender(client, storageData, items);
                    break;
            }
        }

        private void UpdateInventoryRender(Client client, StorageData storageData, object items)
        {
            _uiService.SendMessage(client, "storage", "render_inventory", new
            {
                weight = CalculateWeight(storageData).ToString("0.00"),
                maxWeight = storageData.MaxWeight,
                items
            });
        }

        private void UpdateChestRender(Client client, StorageData storageData, object items)
        {
            _uiService.SendMessage(client, "storage", "render_chest", new
            {
                weight = CalculateWeight(storageData).ToString("0.00"),
                maxWeight = storageData.MaxWeight,
                items
            });
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
