using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Models;
using Average.Server.Repositories;
using Average.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class InventoryService : IService
    {
        private readonly InventoryRepository _repository;
        private readonly EventService _eventService;
        private readonly ClientService _clientService;

        private readonly List<StorageItemInfo> _items = new();
        private readonly Dictionary<string, StorageData> _clients = new();

        public const double DefaultMaxInventoryWeight = 20.0;
        public const double DefaultMaxChestWeight = 100.0;

        public InventoryService(ClientService clientService, InventoryRepository repository, EventService eventService)
        {
            _repository = repository;
            _eventService = eventService;
            _clientService = clientService;

            Logger.Write("InventoryService", "Initialized successfully");
        }

        internal async void OnClientInitialized(Client client, string characterId)
        {
            try
            {
                var storage = await Get(characterId);

                if (storage == null)
                {
                    Logger.Error($"Unable to get inventory for client: {client.Name}: {characterId}");
                    return;
                }

                SetLocalStorage(client, storage);

                _eventService.EmitClient(client, "inventory:init", _items.ToJson(), storage.ToJson());
            }
            catch (Exception ex)
            {
                Logger.Error("Ex: " + ex.InnerException);
            }
        }

        internal bool ClientLocalStorageExists(Client client)
        {
            return _clients.ContainsKey(client.License);
        }

        internal void SetLocalStorage(Client client, StorageData value)
        {
            if (!ClientLocalStorageExists(client))
            {
                _clients.Add(client.License, value);
            }
            else
            {
                _clients[client.License] = value;
            }
        }

        internal StorageData GetLocalStorage(Client client)
        {
            if (ClientLocalStorageExists(client))
            {
                return _clients[client.License];
            }
            else
            {
                return null;
            }
        }

        internal void RegisterItem(StorageItemInfo itemInfo)
        {
            _items.Add(itemInfo);
        }

        internal async Task<bool> SaveInventory(StorageData storageData)
        {
            return await Update(storageData);
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

        internal bool HasFreeSpaceForWeight(StorageItemData itemData, StorageData storageData)
        {
            var info = GetItemInfo(itemData.Name);
            var totalNeededWeight = CalculateWeight(storageData) + itemData.Count * info.Weight;

            //if (totalNeededWeight > storageData.MaxWeight)
            //{
            //    if (info.CanBeStacked && info.OnStacking != null)
            //    {
            //        return true;
            //    }
            //}

            return totalNeededWeight <= storageData.MaxWeight;
        }

        internal void GiveItem(Client target, string itemName, int itemCount)
        {
            _eventService.EmitClient(target, "inventory:giveitem", itemName, itemCount);
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

        internal bool IsSlotAvailable(StorageData storageData, int slotIndex)
        {
            return storageData.Items.Exists(x => x.SlotId == slotIndex);
        }

        internal bool IsSlotExistsWithItemName(StorageData storageData, string itemName)
        {
            return storageData.Items.Exists(x => x.Name == itemName);
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