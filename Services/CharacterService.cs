using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Repositories;
using Average.Shared.DataModels;
using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class CharacterService : IService
    {
        private readonly CharacterRepository _repository;
        private readonly EventService _eventManager;

        public CharacterService(CharacterRepository repository, EventService eventManager)
        {
            _repository = repository;
            _eventManager = eventManager;
        }

        public async Task<IEnumerable<CharacterData>> GetAll() => _repository.GetAll();
        public async Task<CharacterData> Get(long characterId, bool includeChild = false) => _repository.GetAll(includeChild).Find(x => x.Id == characterId);
        public async Task<CharacterData> Get(Player player, bool includeChild = false) => _repository.GetAll(includeChild).Find(x => x.License == player.License());
        public async Task<CharacterData> Get(string license, bool includeChild = false) => _repository.GetAll(includeChild).Find(x => x.License == license);
        public async Task<CharacterData> GetByUserId(long userId, bool includeChild = false) => _repository.GetAll(includeChild).Find(x => x.UserId == userId);
        public async Task<List<CharacterData>> GetPlayerCharacters(Player player) => _repository.GetAll().Where(x => x.License == player.License()).ToList();

        public async void Create(UserData userData, CharacterData characterData)
        {
            try
            {
                var exists = await ExistsByUserId(userData.Id);

                if (!exists)
                {
                    characterData.UserId = userData.Id;
                    await _repository.Add(characterData);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[CharacterService] Unable to create character for player {characterData.License}. Error: {ex.Message}\n{ex.InnerException}.");
            }
        }

        public async void Update(CharacterData data) => await _repository.Update(data);
        public async void Delete(CharacterData data) => await _repository.Delete(data.Id);
        public async Task<bool> Exists(Player player) => await Get(player) != null;
        public async Task<bool> Exists(long characterId) => await Get(characterId) != null;
        public async Task<bool> Exists(string license) => await Get(license) != null;
        public async Task<bool> ExistsByUserId(long userId) => await GetByUserId(userId) != null;

        internal void OnSetPed(Client client, uint model, int variation) => _eventManager.EmitClient(client, "character:set_ped", model, variation);

        internal async void OnLoadAppearance(Client client)
        {
            var characterData = await Get(client);
            _eventManager.EmitClient(client, "character:set_appearance", characterData.ToJson());
        }

        internal void OnRemoveAllClothes(Client client) => _eventManager.EmitClient(client, "character:remove_all_clothes");

        internal void OnSetPedOutfit(Client client, Dictionary<string, uint> outfit) => _eventManager.EmitClient(client, "character:set_outfit", outfit.ToJson());

        internal async void OnSpawnPed(Client client)
        {
            var characterData = await Get(client, true);
            Logger.Debug("character1: " + characterData.ToJson(Formatting.Indented));
            _eventManager.EmitClient(client, "character:spawn_ped", characterData.ToJson());
        }

        internal async void OnSetMoney(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Money = amount;
            Update(data);
        }

        internal async void OnSetBank(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Bank = amount;
            Update(data);
        }

        internal async void OnAddMoney(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Money += amount;
            Update(data);
        }

        internal async void OnAddBank(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Bank += amount;
            Update(data);
        }

        internal async void OnRemoveMoney(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Money -= amount;
            Update(data);
        }

        internal async void OnRemoveBank(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Bank -= amount;
            Update(data);
        }
    }
}
