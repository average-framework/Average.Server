using Average.Server.DataModels;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Model;
using Average.Server.Repositories;
using CitizenFX.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class CharacterService : IService
    {
        private readonly CharacterRepository _repository;
        private readonly EventManager _eventManager;
        private readonly Dictionary<string, CharacterData> _characters = new Dictionary<string, CharacterData>();

        public object Log { get; internal set; }

        public CharacterService(CharacterRepository repository, EventManager eventManager)
        {
            _repository = repository;
            _eventManager = eventManager;

            Logger.Write("CharacterService", "Initialized successfully");
        }

        public async Task<IEnumerable<CharacterData>> GetAll() => _repository.GetAll();
        public async Task<CharacterData> Get(long characterId) => _repository.GetAll().FirstOrDefault(x => x.Id == characterId);
        public async Task<CharacterData> Get(Player player) => _repository.GetAll().FirstOrDefault(x => x.License == player.License());
        public async Task<List<CharacterData>> GetPlayerCharacters(Player player) => _repository.GetAll().Where(x => x.License == player.License()).ToList();
        public async void Update(CharacterData data) => await _repository.Update(data);
        public async void Delete(CharacterData data) => await _repository.Delete(data.Id);
        public async Task<bool> Exists(Player player) => await Get(player) != null;
        public async Task<bool> Exists(long characterId) => await Get(characterId) != null;

        internal void OnSetPed(Client client, uint model, int variation) => _eventManager.EmitClient(client, "character:set_ped", model, variation);

        internal async void OnLoadAppearance(Client client)
        {
            var character = await Get(client);
            var face = character.Face.ToJson();
            var overlays = character.FaceOverlays.ToJson();
            var texture = character.Texture.ToJson();
            var clothes = character.Clothes.ToJson();

            _eventManager.EmitClient(client, "character:set_appearance", (int)character.Gender, character.Origin, character.Head, character.Body, character.BodyType, character.WaistType, character.Legs, character.Scale, texture, face, overlays, clothes);
        }

        internal void OnRemoveAllClothes(Client client) => _eventManager.EmitClient(client, "character:remove_all_clothes");

        internal void OnSetPedOutfit(Client client, Dictionary<string, uint> outfit) => _eventManager.EmitClient(client, "character:set_outfit", outfit.ToJson());

        internal async void OnRespawnPed(Client client)
        {
            var character = await Get(client);
            _eventManager.EmitClient(client, "character:respawn_ped", (int)character.Gender);
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
