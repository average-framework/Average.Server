using Average.Server.Data;
using CitizenFX.Core;
using Newtonsoft.Json;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Server.Rpc;
using SDK.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class CharacterManager : ICharacterManager, ISaveable
    {
        Logger logger;
        SQL sql;
        PlayerList players;

        const string tableName = "characters";

        Dictionary<string, CharacterData> characters { get; } = new Dictionary<string, CharacterData>();

        public CharacterManager(Logger logger, RpcRequest rpc, SQL sql, EventManager eventManager, PlayerList players, EventHandlerDictionary eventHandler, SaveManager save)
        {
            this.logger = logger;
            this.sql = sql;
            this.players = players;

            #region Rpc

            rpc.Event("Character.Exist").On(async (message, callback) =>
            {
                try
                {
                    var player = players[message.Target];
                    var characterExist = await Exist(player);
                    callback(characterExist.ToString().ToLower());

                    logger.Debug($"[Character] Getted character exist: [{characterExist}]");
                }
                catch
                {
                    logger.Debug("[Character] Unable to check if character exist.");
                }
            });

            rpc.Event("Character.Load").On(async (message, callback) =>
            {
                try
                {
                    var player = players[message.Target];
                    var data = await Load(player.Identifiers["license"]);
                    callback(data);

                    logger.Debug($"[Character] Getted character");
                }
                catch
                {
                    logger.Debug("[Character] Unable get character.");
                }
            });

            #endregion

            #region Event

            eventManager.PlayerDisconnecting += PlayerDisconnecting;

            eventHandler["Character.SetPed"] += new Action<int, uint, int>(OnSetPedEvent);
            eventHandler["Character.Save"] += new Action<int, string>(OnSaveEvent);
            eventHandler["Character.SetMoney"] += new Action<int, decimal>(OnSetMoneyEvent);
            eventHandler["Character.SetBank"] += new Action<int, decimal>(OnSetBankEvent);
            eventHandler["Character.AddMoney"] += new Action<int, decimal>(OnAddMoneyEvent);
            eventHandler["Character.AddBank"] += new Action<int, decimal>(OnAddBankEvent);
            eventHandler["Character.RemoveMoney"] += new Action<int, decimal>(OnRemoveMoneyEvent);
            eventHandler["Character.RemoveBank"] += new Action<int, decimal>(OnRemoveBankEvent);

            #endregion

            save.AddInQueue(this);
        }

        public CharacterData GetCache(string rockstarId)
        {
            if (characters.ContainsKey(rockstarId))
                return characters[rockstarId];

            return null;
        }

        public async Task<CharacterData> Load(string rockstarId)
        {
            var data = await sql.GetAllAsync<CharacterData>(tableName, x => x.RockstarId == rockstarId);

            if (!characters.ContainsKey(rockstarId))
                characters.Add(rockstarId, data[0]);
            else
                characters[rockstarId] = data[0];

            return data[0];
        }

        public async Task<bool> CacheExist(Player player, bool isLocal)
        {
            if (isLocal)
                return characters.Values.ToList().Exists(x => x.RockstarId == player.Identifiers["license"]);
            else
                return await sql.ExistsAsync<CharacterData>(tableName, x => x.RockstarId == player.Identifiers["license"]);
        }

        public async Task<bool?> Exist(Player player) => await sql.ExistsAsync<CharacterData>(tableName, x => x.RockstarId == player.Identifiers["license"]);

        public async Task Create(CharacterData data) => await sql.InsertOrUpdateAsync(tableName, data);

        public async Task Save(Player player)
        {
            var rockstarId = player.Identifiers["license"];

            if (characters.ContainsKey(rockstarId))
            {
                try
                {
                    var data = characters[rockstarId];
                    await sql.InsertOrUpdateAsync(tableName, data);
                    logger.Debug("[Character] Saved: " + rockstarId);
                }
                catch (Exception ex)
                {
                    logger.Error($"[Character] Error on saving character: {rockstarId}. Error: " + ex.Message);
                }
            }
        }

        public async Task SaveAll()
        {
            for (int i = 0; i < characters.Count; i++)
            {
                var data = characters.ElementAt(i);

                try
                {
                    await sql.InsertOrUpdateAsync(tableName, data.Value);
                    logger.Debug("[Character] Saved: " + data.Key);
                }
                catch (Exception ex)
                {
                    logger.Error($"[Character] Error on saving character: {data.Key}. Error: " + ex.Message);
                }
            }
        }

        public void UpdateCache(Player player, CharacterData data)
        {
            var rockstarId = player.Identifiers["license"];

            if (characters.ContainsKey(rockstarId))
            {
                characters[rockstarId] = data;
                logger.Debug("[Character] Cache updated: " + rockstarId);
            }
        }

        #region Event

        protected void OnSetPedEvent(int player, uint model, int variation) => players[player].TriggerEvent("Character.SetPed", model, variation);

        protected async void OnSaveEvent(int player, string json)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json))
            {
                logger.Error("[Character] Unable to save character for player: " + players[player].Name + ", json contains an error.");
                return;
            }

            UpdateCache(players[player], JsonConvert.DeserializeObject<CharacterData>(json));
        }

        protected async void PlayerDisconnecting(object sender, SDK.Server.Events.PlayerDisconnectingEventArgs e) => await Save(e.Player);

        protected void OnSetMoneyEvent(int player, decimal amount) => players[player].TriggerEvent("Character.SetMoney", amount);

        protected void OnSetBankEvent(int player, decimal amount) => players[player].TriggerEvent("Character.SetBank", amount);

        protected void OnAddMoneyEvent(int player, decimal amount) => players[player].TriggerEvent("Character.AddMoney", amount);

        protected void OnAddBankEvent(int player, decimal amount) => players[player].TriggerEvent("Character.AddBank", amount);

        protected void OnRemoveMoneyEvent(int player, decimal amount) => players[player].TriggerEvent("Character.RemoveMoney", amount);

        protected void OnRemoveBankEvent(int player, decimal amount) => players[player].TriggerEvent("Character.RemoveBank", amount);

        #endregion
    }
}
