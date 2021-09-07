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

namespace Average.Server.Managers
{
    public class CharacterManager : InternalPlugin, ICharacterManager, ISaveable
    {
        public const string TableName = "characters";

        private readonly Dictionary<string, CharacterData> _characters = new Dictionary<string, CharacterData>();
        
        public override void OnInitialized()
        {
            #region Rpc

            Rpc.Event("Character.Exist").On(async (message, callback) =>
            {
                try
                {
                    var player = Players[message.Target];
                    var characterExist = await Exist(player);
                    callback(characterExist.ToString().ToLower());

                    Log.Debug($"[Character] Getted character exist: [{characterExist}]");
                }
                catch
                {
                    Log.Debug("[Character] Unable to check if character exist.");
                }
            });

            Rpc.Event("Character.Load").On(async (message, callback) =>
            {
                try
                {
                    var player = Players[message.Target];
                    var data = await Load(player.Identifiers["license"]);
                    callback(data);

                    Log.Debug($"[Character] Getted character: {data.RockstarId}.");
                }
                catch
                {
                    Log.Debug("[Character] Unable get character.");
                }
            });

            #endregion

            #region Event

            Event.PlayerDisconnecting += PlayerDisconnecting;

            #endregion

            Save.AddInQueue(this);
        }

        public CharacterData? GetCache(string rockstarId)
        {
            if (_characters.ContainsKey(rockstarId))
                return _characters[rockstarId];

            return null;
        }

        public async Task<CharacterData> Load(string rockstarId)
        {
            if (_characters.ContainsKey(rockstarId))
            {
                return _characters[rockstarId];
            }
            else
            {
                var data = await Sql.GetAllAsync<CharacterData>(TableName, $"RockstarId=\"{rockstarId}\"");

                if (!_characters.ContainsKey(rockstarId))
                {
                    _characters.Add(rockstarId, data[0]);
                }
                else
                {
                    _characters[rockstarId] = data[0];
                }

                return data[0];
            }

            return null;
        }

        public async Task<bool> CacheExist(Player player, bool isLocal)
        {
            if (isLocal)
                return _characters.Values.ToList().Exists(x => x.RockstarId == player.Identifiers["license"]);
            else
                return await Sql.ExistsAsync<CharacterData>(TableName, x => x.RockstarId == player.Identifiers["license"]);
        }

        public async Task<bool?> Exist(Player player) => await Sql.ExistsAsync<CharacterData>(TableName, x => x.RockstarId == player.Identifiers["license"]);

        public async Task Create(CharacterData data) => await Sql.InsertOrUpdateAsync(TableName, data);

        public async Task SaveData(Player player)
        {
            var rockstarId = player.Identifiers["license"];

            if (_characters.ContainsKey(rockstarId))
            {
                try
                {
                    var data = _characters[rockstarId];
                    await Sql.InsertOrUpdateAsync(TableName, data);
                    Log.Debug("[Character] Saved: " + rockstarId);
                }
                catch (Exception ex)
                {
                    Log.Error($"[Character] Error on saving character: {rockstarId}. Error: " + ex.Message);
                }
            }
        }

        public async Task SaveAll()
        {
            for (int i = 0; i < _characters.Count; i++)
            {
                var data = _characters.ElementAt(i);
                var cache = GetCache(data.Value.RockstarId);
                if (cache == null) return;

                try
                {
                    await Sql.InsertOrUpdateAsync(TableName, cache);
                    Log.Debug("[Character] Saved: " + data.Key);
                }
                catch (Exception ex)
                {
                    Log.Error($"[Character] Error on saving character: {data.Key}. Error: " + ex.Message);
                }
            }
        }

        public void UpdateCache(Player player, CharacterData data)
        {
            var rockstarId = player.Identifiers["license"];

            if (_characters.ContainsKey(rockstarId))
            {
                _characters[rockstarId] = data;
                Log.Debug("[Character] Cache updated: " + rockstarId);
            }
            else
            {
                _characters.Add(rockstarId, data);
            }
        }

        #region Event

        [ServerEvent("Character.SavePosition")]
        private void SavePositionEvent(int player, Vector3 coords, float heading)
        {
            var p = Players[player];
            if (p == null) return;
            var cache = GetCache(p.Identifiers["license"]);
            if (cache == null) return;
            cache.Position.X = coords.X;
            cache.Position.Y = coords.Y;
            cache.Position.Z = coords.Z;
            cache.Position.H = heading;
            
            UpdateCache(p, cache);
        }
        
        [ServerEvent("Character.SetPed")]
        private void OnSetPedEvent(int player, uint model, int variation) => Players[player].TriggerEvent("Character.SetPed", model, variation);

        [ServerEvent("Character.Create")]
        private async void OnCreateEvent(int player, string json)
        {
            var p = Players[player];
            if (p == null) return;
            var character = JsonConvert.DeserializeObject<CharacterData>(json);
            UpdateCache(p, character);
            await Sql.InsertAsync(TableName, character);
        }
        
        [ServerEvent("Character.Save")]
        private void OnSaveEvent(int player, string json)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json))
            {
                Log.Error("[Character] Unable to save character for player: " + Players[player].Name + ", json contains an error.");
                return;
            }

            UpdateCache(Players[player], JsonConvert.DeserializeObject<CharacterData>(json));
        }

        private async void PlayerDisconnecting(object sender, SDK.Server.Events.PlayerDisconnectingEventArgs e) => await SaveData(e.Player);

        [ServerEvent("Character.SetMoney")]
        private void OnSetMoneyEvent(int player, decimal amount) => Players[player].TriggerEvent("Character.SetMoney", amount);

        [ServerEvent("Character.SetBank")]
        private void OnSetBankEvent(int player, decimal amount) => Players[player].TriggerEvent("Character.SetBank", amount);

        [ServerEvent("Character.AddMoney")]
        private void OnAddMoneyEvent(int player, decimal amount) => Players[player].TriggerEvent("Character.AddMoney", amount);

        [ServerEvent("Character.AddBank")]
        private void OnAddBankEvent(int player, decimal amount) => Players[player].TriggerEvent("Character.AddBank", amount);

        [ServerEvent("Character.RemoveMoney")]
        private void OnRemoveMoneyEvent(int player, decimal amount) => Players[player].TriggerEvent("Character.RemoveMoney", amount);

        [ServerEvent("Character.RemoveBank")]
        private void OnRemoveBankEvent(int player, decimal amount) => Players[player].TriggerEvent("Character.RemoveBank", amount);

        #endregion
    }
}
