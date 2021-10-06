using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Repositories;
using Average.Shared.DataModels;
using CitizenFX.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class UserService : IService
    {
        private readonly UserRepository _repository;

        public UserService(UserRepository repository)
        {
            _repository = repository;

            Logger.Write("UserService", "Initialized successfully");
        }

        public async Task<List<UserData>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<UserData> Get(Player player) => await _repository.GetAsync(x => x.License == player.License());
        public async Task<UserData> Get(string license) => await _repository.GetAsync(x => x.License == license);
        public async Task<bool> Update(UserData data) => await _repository.ReplaceOneAsync(x => x.Id, data.Id, data);
        public async Task<bool> Update(Expression<Func<UserData, bool>> expression, params UpdateDefinition<UserData>[] definitions) => await _repository.UpdateOneAsync(expression, definitions);
        public async Task<bool> Delete(UserData data) => await _repository.DeleteOneAsync(x => x.Id == data.Id);
        public async Task<bool> Exists(Player player) => await _repository.ExistsAsync(x => x.License == player.License());
        public async Task<bool> Exists(string license) => await _repository.ExistsAsync(x => x.License == license);

        public async Task<bool> Create(Player player) => await _repository.AddAsync(new UserData
        {
            License = player.License(),
            IsBanned = false,
            IsWhitelisted = false,
            IsConnected = false,
            CreatedAt = DateTime.Now,
            LastConnection = DateTime.Now,
            PermissionLevel = 0
        });

        public async void UpdateLastConnectionTime(UserData user)
        {
            await Update(x => x.Id == user.Id, _repository.USet(x => x.LastConnection, DateTime.Now));
        }
        public async void UpdateConnectionState(UserData user, bool isConnected)
        {
            await Update(x => x.Id == user.Id, _repository.USet(x => x.IsConnected, isConnected));
        }

        public async Task<DateTime> GetLastConnectionTime(Player player)
        {
            var result = await Get(player);
            return result.LastConnection.ToLocalTime();
        }
    }
}
