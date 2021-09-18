using Average.Server.Framework.Extensions;
using Average.Server.Repositories;
using CitizenFX.Core;
using SDK.Server.Diagnostics;
using SDK.Server.Extensions;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    public class UserService : IService
    {
        private readonly UserRepository _repository;

        public UserService(UserRepository repository)
        {
            _repository = repository;

            Logger.Write("UserService", "Initialized successfully");
        }

        public async Task<IEnumerable<UserData>> GetAll() => _repository.GetAll();
        public async Task<UserData> Get(long userId) => _repository.GetAll().FirstOrDefault(x => x.Id == userId);
        public async Task<UserData> Get(Player player) => _repository.GetAll().FirstOrDefault(x => x.Licence == player.License());
        public async void Update(UserData data) => await _repository.Update(data);
        public async void Delete(UserData data) => await _repository.Delete(data.Id);
        public async Task<bool> Exists(Player player) => await Get(player) != null;
        public async Task<bool> Exists(long userId) => await Get(userId) != null;

        public async void Create(Player player)
        {
            await _repository.Add(new UserData
            {
                Licence = player.License(),
                IsBanned = 0,
                IsWhitelisted = 0,
                IsConnected = 0,
                CreatedAt = DateTime.Now,
                LastConnection = DateTime.Now,
                PermissionLevel = 0
            });
        }

        public void UpdateLastConnectionTime(UserData user) => user.LastConnection = DateTime.Now;
        public void UpdateConnectionState(UserData user, bool isConnected) => user.IsConnected = isConnected ? 1 : 0;

        public async Task<DateTime> GetLastConnectionTime(Player player)
        {
            var result = await Get(player);
            return result.LastConnection.ToLocalTime();
        }
    }
}
