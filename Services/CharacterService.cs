using CitizenFX.Core;
using Newtonsoft.Json;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SDK.Server;
using Average.Server.Repositories;
using SDK.Server.Extensions;

namespace Average.Server.Managers
{
    public class CharacterService : ICharacterManager, IService
    {
        private readonly Dictionary<string, CharacterData> _characters = new Dictionary<string, CharacterData>();
        private readonly CharacterRepository _repository;

        public object Log { get; internal set; }

        public CharacterService(CharacterRepository repository)
        {
            _repository = repository;

            Logger.Write("CharacterService", "Initialized successfully");

            //Save.AddInQueue(this);
        }

        public async Task<IEnumerable<CharacterData>> GetAll() => _repository.GetAll();
        public async Task<CharacterData> Get(long characterId) => _repository.GetAll().FirstOrDefault(x => x.Id == characterId);
        public async Task<CharacterData> Get(Player player) => _repository.GetAll().FirstOrDefault(x => x.License == player.License());
        public async Task<List<CharacterData>> GetPlayerCharacters(Player player) => _repository.GetAll().Where(x => x.License == player.License()).ToList();
        public async void Update(CharacterData data) => await _repository.Update(data);
        public async void Delete(CharacterData data) => await _repository.Delete(data.Id);
        public async Task<bool> Exists(Player player) => await Get(player) != null;
        public async Task<bool> Exists(long characterId) => await Get(characterId) != null;
    }
}
