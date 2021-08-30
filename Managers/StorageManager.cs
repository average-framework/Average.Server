using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using SDK.Server;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Server.Models;
using SDK.Server.Rpc;
using SDK.Shared.DataModels;
using SDK.Shared.Rpc;

namespace Average.Server.Managers
{
    public class StorageManager : InternalPlugin, IStorageManager, ISaveable
    {
        private const string tableName = "storages";
        
        private readonly Dictionary<string, StorageData> _storages = new Dictionary<string, StorageData>();

        public static List<StorageItemInfo> RegisteredItems { get; private set; }
        
        public override void OnInitialized()
        {
            RegisteredItems = SDK.Server.Configuration.Parse<List<StorageItemInfo>>("configs/storage_items.json");

            #region Event

            Rpc.Event("Storage.GetRegisteredItems").On(OnGetRegisteredItemsEvent);
            Rpc.Event("Storage.GetInventory").On<string>(OnGetInventoryEvent);
            Rpc.Event("Storage.GetChest").On<string>(OnGetChestEvent);
            Rpc.Event("Storage.HasFreeSpace").On<int>(OnHasFreeSpaceEvent);
            
            #endregion
        }

        public StorageItemInfo GetItemInfo(string itemName)
        {
            return RegisteredItems.Find(x => x.Name == itemName);
        }
        
        public double CalculateWeight(StorageData storage)
        {
            var weight = 0d;
            storage.Items.ForEach(x => weight += x.Count * GetItemInfo(x.Name).Weight);
            return weight;
        }
        
        public bool HasFreeSpace(StorageData storage) => CalculateWeight(storage) <= storage.MaxWeight;

        public StorageData? GetCache(string storageId)
        {
            if (_storages.ContainsKey(storageId))
            {
                return _storages[storageId];
            }
            
            return null;
        }

        public async Task<bool> CacheExist(string storageId, bool isLocal)
        {
            if (isLocal)
            {
                return _storages.Values.ToList().Exists(x => x.StorageId == storageId);
            }
            else
            {
                return await Sql.ExistsAsync<StorageData>(tableName, x => x.StorageId == storageId);
            }
        }

        public async Task<bool?> Exist(string storageId) => await Sql.ExistsAsync<StorageData>(tableName, x => x.StorageId == storageId);
        
        public async Task Create(StorageData data) => await Sql.InsertOrUpdateAsync(tableName, data);
        
        public async Task<StorageData> Load(string storageId)
        {
            var data = await Sql.GetAllAsync<StorageData>(tableName, x => x.StorageId == storageId);

            if (!_storages.ContainsKey(storageId))
            {
                _storages.Add(storageId, data[0]);
            }
            else
            {
                _storages[storageId] = data[0];
            }

            return data[0];
        }
        
        public async Task SaveData(Player player)
        {
            var license = player.Identifiers["license"];

            if (_storages.ContainsKey(license))
            {
                try
                {
                    var data = _storages[license];
                    await Sql.InsertOrUpdateAsync(tableName, data);
                    Log.Debug($"[Storage] Saved: {license}.");
                }
                catch (Exception ex)
                {
                    Log.Error($"[Storage] Error on saving storage: {license}. Error: {ex.Message}.");
                }
            }
        }
        
        public async Task SaveAll()
        {
            for (int i = 0; i < _storages.Count; i++)
            {
                var data = _storages.ElementAt(i);

                try
                {
                    await Sql.InsertOrUpdateAsync(tableName, data.Value);
                    Log.Debug($"[Storage] Saved: {data.Key}.");
                }
                catch (Exception ex)
                {
                    Log.Error($"[Storage] Error on saving storage: {data.Key}. Error: {ex.Message}.");
                }
            }
        }
        
        public void UpdateCache(Player player, StorageData data)
        {
            var license = player.Identifiers["license"];

            if (_storages.ContainsKey(license))
            {
                _storages[license] = data;
                Log.Debug($"[Character] Cache updated: {license}.");
            }
        }
        
        #region Event

        [ServerEvent("Storage.Remove")]
        private async void OnRemoveEvent(int player, string storageId)
        {
            try
            {
                await Sql.DeleteAllAsync<StorageData>("storages", x => x.StorageId == storageId);
            }
            catch (Exception ex)
            {
                Log.Error($"$[Storage] Unable to delete storage. Error: {ex.Message}.");
            }
        }
        
        [ServerEvent("Storage.GiveItemToPlayer")]
        private void OnGiveItemToPlayerEvent(int player, int targetServerId, string itemJson, int itemCount)
        {
            Players[targetServerId].TriggerEvent("Storage.GiveItemToPlayer", itemJson, itemCount);
        }

        [ServerEvent("Storage.RemoveItem")]
        private void OnRemoveItemEvent(int player, int targetServerId, string itemName, int itemCount)
        {
            Players[targetServerId].TriggerEvent("Storage.RemoveItem", itemName, itemCount);
        }
        
        [ServerEvent("Storage.RemoveItemById")]
        private void OnRemoveItemByIdEvent(int player, int targetServerId, string itemId, int itemCount)
        {
            Players[targetServerId].TriggerEvent("Storage.RemoveItemById", itemId, itemCount);
        }

        [ServerEvent("Storage.GiveMoneyToPlayer")]
        private void OnGiveMoneyToPlayerEvent(int player, int targetServerId, string amount)
        {
            Event.EmitClient(Players[targetServerId], "Storage.GiveMoneyToPlayer", amount);
        }
        
        [ServerEvent("Storage.Save")]
        private async void OnSaveEvent(int player, string json)
        {
            var storage = JsonConvert.DeserializeObject<StorageData>(json);

            if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json))
            {
                Log.Error($"[Storage] Unable to save storage for player: {Players[player].Name}. Json have an invalid format.");
                return;
            }

            UpdateCache(Players[player], storage);
            await BaseScript.Delay(0);
            Event.EmitClients("Storage.Updated", storage.StorageId);
        }
        
        [ServerEvent("Storage.RefreshChest")]
        private void OnRefreshChestEvent(int player, string storageId)
        {
            Event.EmitClients("Storage.RefreshChest", storageId);
        }
        
        [ServerEvent("PlayerDisconnecting")]
        private async void OnPlayerDisconnectingEvent(int player, string reason) => await SaveData(Players[player]);

        #endregion

        #region Rpc

        private void OnGetRegisteredItemsEvent(RpcMessage message, RpcRequest.RpcCallback callback) => callback(RegisteredItems);

        private async void OnGetInventoryEvent(string license, RpcRequest.RpcCallback callback)
        {
            try
            {
                var storage = await Load(license);
                while (storage is null) await BaseScript.Delay(0);
                callback(storage);
            }
            catch (Exception ex)
            {
                Log.Error($"[Storage] Unable to load inventory. This license exist ? [{license}]. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }
        
        private async void OnGetChestEvent(string storageId, RpcRequest.RpcCallback callback)
        {
            try
            {
                var storage = await Load(storageId);
                while (storage is null) await BaseScript.Delay(0);
                callback(storage);
            }
            catch (Exception ex)
            {
                Log.Error($"[Storage] Unable to load chest. This storage id exist ? [{storageId}]. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }

        private void OnHasFreeSpaceEvent(int targetServerId, RpcRequest.RpcCallback callback)
        {
            try
            {
                var target = Players[targetServerId];
                var license = target.Identifiers["license"];

                if (target != null)
                {
                    var cache = GetCache("player_" + license);

                    if (cache != null)
                    {
                        callback(HasFreeSpace(cache), cache.StorageId);
                    }
                    else
                    {
                        Log.Error($"[Storage] This cache doesn't exist: [{cache.StorageId}].");
                    }
                }
                else
                {
                    Log.Error($"[Storage] This target doesn't exist: [{targetServerId}]");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Storage] Unable to call OnHasFreeSpaceEvent. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }

        #endregion
    }
}