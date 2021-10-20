using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Repositories;
using Average.Shared.DataModels;
using CitizenFX.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

            Logger.Write("CharacterService", "Initialized successfully");
        }

        public async Task<IEnumerable<CharacterData>> GetAll() => await _repository.GetAllAsync();
        public async Task<CharacterData> Get(Player player) => await _repository.GetAsync(x => x.License == player.License());
        public async Task<CharacterData> Get(string license) => await _repository.GetAsync(x => x.License == license);
        public async Task<CharacterData> GetByCharacterId(string characterId) => await _repository.GetAsync(x => x.CharacterId == characterId);
        public async Task<List<CharacterData>> GetCharacters(Player player) => await _repository.GetAllAsync(x => x.License == player.License());
        public async Task<bool> Update(CharacterData data) => await _repository.ReplaceOneAsync(x => x.Id, data.Id, data);
        public async Task<bool> Update(Expression<Func<CharacterData, bool>> expression, params UpdateDefinition<CharacterData>[] definitions) => await _repository.UpdateOneAsync(expression, definitions);
        public async Task<bool> Delete(CharacterData data) => await _repository.DeleteOneAsync(x => x.Id == data.Id);
        public async Task<bool> Exists(Player player) => await _repository.ExistsAsync(x => x.License == player.License());
        public async Task<bool> Exists(string license) => await _repository.ExistsAsync(x => x.License == license);

        public async void Create(CharacterData characterData)
        {
            try
            {
                var exists = await _repository.ExistsAsync(x => x.License == characterData.License);

                if (!exists)
                {
                    var added = await _repository.AddAsync(characterData);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[CharacterService] Unable to create character for player {characterData.License}. Error: {ex.Message}\n{ex.StackTrace}.");
            }
        }

        public async void UpdatePosition(CharacterData character, PositionData position)
        {
            await Update(x => x.Id == character.Id, _repository.USet(x => x.Position, position));
        }

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
            var characterData = await Get(client);
            _eventManager.EmitClient(client, "character:spawn_ped", characterData.ToJson(Formatting.None));
        }

        internal void OnTeleport(Client client, Vector3 position)
        {
            _eventManager.EmitClient(client, "character:teleport", position);
        }

        internal async void OnSetMoney(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Money = amount;
            await Update(data);
        }

        internal async void OnSetBank(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Bank = amount;
            await Update(data);
        }

        internal async void OnAddMoney(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Money += amount;
            await Update(data);
        }

        internal async void OnAddBank(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Bank += amount;
            await Update(data);
        }

        internal async void OnRemoveMoney(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Money -= amount;
            await Update(data);
        }

        internal async void OnRemoveBank(Client client, decimal amount)
        {
            var data = await Get(client);
            data.Economy.Bank -= amount;
            await Update(data);
        }
    }
}
