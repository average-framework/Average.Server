using CitizenFX.Core;
using Newtonsoft.Json;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class CharacterManager : ICharacterManager, ISaveable
    {
        private const string tableName = "characters";

        private Dictionary<string, CharacterData> _characters = new Dictionary<string, CharacterData>();

        public CharacterManager()
        {
            #region Rpc

            Main.rpc.Event("Character.Exist").On(async (message, callback) =>
            {
                try
                {
                    var player = Main.players[message.Target];
                    var characterExist = await Exist(player);
                    callback(characterExist.ToString().ToLower());

                    Log.Debug($"[Character] Getted character exist: [{characterExist}]");
                }
                catch
                {
                    Log.Debug("[Character] Unable to check if character exist.");
                }
            });

            Main.rpc.Event("Character.Load").On(async (message, callback) =>
            {
                try
                {
                    var player = Main.players[message.Target];
                    var data = await Load(player.Identifiers["license"]);
                    callback(data);

                    Log.Debug($"[Character] Getted character");
                }
                catch
                {
                    Log.Debug("[Character] Unable get character.");
                }
            });

            #endregion

            #region Event

            Main.eventManager.PlayerDisconnecting += PlayerDisconnecting;

            Main.eventHandlers["Character.SetPed"] += new Action<int, uint, int>(OnSetPedEvent);
            Main.eventHandlers["Character.Save"] += new Action<int, string>(OnSaveEvent);
            Main.eventHandlers["Character.SetMoney"] += new Action<int, decimal>(OnSetMoneyEvent);
            Main.eventHandlers["Character.SetBank"] += new Action<int, decimal>(OnSetBankEvent);
            Main.eventHandlers["Character.AddMoney"] += new Action<int, decimal>(OnAddMoneyEvent);
            Main.eventHandlers["Character.AddBank"] += new Action<int, decimal>(OnAddBankEvent);
            Main.eventHandlers["Character.RemoveMoney"] += new Action<int, decimal>(OnRemoveMoneyEvent);
            Main.eventHandlers["Character.RemoveBank"] += new Action<int, decimal>(OnRemoveBankEvent);

            #endregion

            Main.saveManager.AddInQueue(this);
        }

        public CharacterData? GetCache(string rockstarId)
        {
            if (_characters.ContainsKey(rockstarId))
                return _characters[rockstarId];

            return null;
        }

        public async Task<CharacterData> Load(string rockstarId)
        {
            var data = await Main.sql.GetAllAsync<CharacterData>(tableName, x => x.RockstarId == rockstarId);

            if (!_characters.ContainsKey(rockstarId))
                _characters.Add(rockstarId, data[0]);
            else
                _characters[rockstarId] = data[0];

            return data[0];
        }

        public async Task<bool> CacheExist(Player player, bool isLocal)
        {
            if (isLocal)
                return _characters.Values.ToList().Exists(x => x.RockstarId == player.Identifiers["license"]);
            else
                return await Main.sql.ExistsAsync<CharacterData>(tableName, x => x.RockstarId == player.Identifiers["license"]);
        }

        public async Task<bool?> Exist(Player player) => await Main.sql.ExistsAsync<CharacterData>(tableName, x => x.RockstarId == player.Identifiers["license"]);

        public async Task Create(CharacterData data) => await Main.sql.InsertOrUpdateAsync(tableName, data);

        public async Task Save(Player player)
        {
            var rockstarId = player.Identifiers["license"];

            if (_characters.ContainsKey(rockstarId))
            {
                try
                {
                    var data = _characters[rockstarId];
                    await Main.sql.InsertOrUpdateAsync(tableName, data);
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

                try
                {
                    await Main.sql.InsertOrUpdateAsync(tableName, data.Value);
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
        }

        #region Event

        private void OnSetPedEvent(int player, uint model, int variation) => Main.players[player].TriggerEvent("Character.SetPed", model, variation);

        private void OnSaveEvent(int player, string json)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json))
            {
                Log.Error("[Character] Unable to save character for player: " + Main.players[player].Name + ", json contains an error.");
                return;
            }

            UpdateCache(Main.players[player], JsonConvert.DeserializeObject<CharacterData>(json));
        }

        private async void PlayerDisconnecting(object sender, SDK.Server.Events.PlayerDisconnectingEventArgs e) => await Save(e.Player);

        private void OnSetMoneyEvent(int player, decimal amount) => Main.players[player].TriggerEvent("Character.SetMoney", amount);

        private void OnSetBankEvent(int player, decimal amount) => Main.players[player].TriggerEvent("Character.SetBank", amount);

        private void OnAddMoneyEvent(int player, decimal amount) => Main.players[player].TriggerEvent("Character.AddMoney", amount);

        private void OnAddBankEvent(int player, decimal amount) => Main.players[player].TriggerEvent("Character.AddBank", amount);

        private void OnRemoveMoneyEvent(int player, decimal amount) => Main.players[player].TriggerEvent("Character.RemoveMoney", amount);

        private void OnRemoveBankEvent(int player, decimal amount) => Main.players[player].TriggerEvent("Character.RemoveBank", amount);

        #endregion
    }
}
