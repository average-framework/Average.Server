using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Repositories;
using Average.Shared.DataModels;
using CitizenFX.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Average.Server.Services.DoorService;

namespace Average.Server.Services
{
    internal class BankService : IService
    {
        private readonly BankRepository _repository;
        private readonly DoorService _doorService;

        public BankService(BankRepository repository, DoorService doorService)
        {
            _repository = repository;
            _doorService = doorService;

            _doorService.Add(new DoorModel(new Vector3(-3666.02f, -2620.88f, -14.57f), 2f, false,
            nearAction: (client, door, isNear) =>
            {
                Logger.Debug($"Player {client.Name} is near of door ?? " + isNear);
            },
            openAction: (client, door) =>
            {
                Logger.Debug($"Player {client.Name} open the door at position: " + door.Position);
            }));

            Logger.Write("BankService", "Initialized successfully");
        }

        public async Task<IEnumerable<BankData>> GetAll() => await _repository.GetAllAsync();
        public async Task<BankData> Get(string characterId) => await _repository.GetAsync(x => x.CharacterId == characterId);
        public async Task<bool> Update(BankData data) => await _repository.ReplaceOneAsync(x => x.CharacterId, data.CharacterId, data);
        public async Task<bool> Update(Expression<Func<BankData, bool>> expression, params UpdateDefinition<BankData>[] definitions) => await _repository.UpdateOneAsync(expression, definitions);
        public async Task<bool> Delete(BankData data) => await _repository.DeleteOneAsync(x => x.CharacterId == data.CharacterId);
        public async Task<bool> Exists(string characterId) => await _repository.ExistsAsync(x => x.CharacterId == characterId);
    }
}
