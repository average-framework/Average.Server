using Average.Server.DataModels;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Repositories;
using CitizenFX.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class CharacterService : IService
    {
        private readonly CharacterRepository _repository;
        private readonly Dictionary<string, CharacterData> _characters = new Dictionary<string, CharacterData>();

        public object Log { get; internal set; }

        public CharacterService(CharacterRepository repository)
        {
            _repository = repository;

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

        private void OnSetPed(Player player, uint model, int variation) => player.TriggerEvent("character:set_ped", model, variation);

        private async void OnSetMoney(Player player, decimal amount)
        {
            var data = await Get(player);
            data.Economy.Money = amount;
            Update(data);
        }

        private async void OnSetBank(Player player, decimal amount)
        {
            var data = await Get(player);
            data.Economy.Bank = amount;
            Update(data);
        }

        private async void OnAddMoney(Player player, decimal amount)
        {
            var data = await Get(player);
            data.Economy.Money += amount;
            Update(data);
        }

        private async void OnAddBank(Player player, decimal amount)
        {
            var data = await Get(player);
            data.Economy.Bank += amount;
            Update(data);
        }

        private async void OnRemoveMoney(Player player, decimal amount)
        {
            var data = await Get(player);
            data.Economy.Money -= amount;
            Update(data);
        }

        private async void OnRemoveBank(Player player, decimal amount)
        {
            var data = await Get(player);
            data.Economy.Bank -= amount;
            Update(data);
        }
    }
}
