using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Repositories;
using Average.Shared.Attributes;
using Average.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Average.Server.Services.RpcService;

namespace Average.Server.Services
{
    internal class InventoryService : IService
    {
        private readonly InventoryRepository _repository;

        public InventoryService(InventoryRepository repository)
        {
            _repository = repository;
        }

        [UICallback("inventorytest")]
        private void OnTest(Dictionary<string, object> args, RpcCallback cb)
        {
            Logger.Error("inventorytest triggered: " + string.Join(", ", args));

            cb("enculer", "enculer 2");
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
