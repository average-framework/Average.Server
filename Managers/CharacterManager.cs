using CitizenFX.Core;
using Newtonsoft.Json;
using SDK.Server;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class CharacterManager : ICharacterManager
    {
        Framework framework;

        public Dictionary<string, CharacterData> Characters { get; } = new Dictionary<string, CharacterData>();

        public CharacterManager(Framework framework)
        {
            this.framework = framework;

            Task.Factory.StartNew(async () => 
            {
                await framework.IsReadyAsync();

                framework.Rpc.Event("Character.Exist").On(async (message, callback) =>
                {
                    try
                    {
                        var player = framework.Players[message.Target];
                        var characterExist = await Exist(player);
                        callback(characterExist.ToString().ToLower());
                    }
                    catch
                    {
                        framework.Logger.Error("[Character] Unable to check if character exist");
                    }
                });

                framework.Event.PlayerDisconnecting += PlayerDisconnecting;
            });
        }

        public CharacterData GetLocal(string rockstarId)
        {
            if (Characters.ContainsKey(rockstarId))
            {
                return Characters[rockstarId];
            }

            return null;
        }

        public async Task<CharacterData> Load(string rockstarId)
        {
            var data = await framework.Sql.GetAllAsync<CharacterData>("characters", x => x.RockstarId == rockstarId);
            return data[0];
        }

        public async Task<bool> LocalExist(Player player, bool isLocal)
        {
            if (isLocal)
            {
                return Characters.Values.ToList().Exists(x => x.RockstarId == player.Identifiers["license"]);
            }
            else
            {
                return await framework.Sql.ExistsAsync<CharacterData>("characters", x => x.RockstarId == player.Identifiers["license"]);
            }
        }

        public async Task<bool?> Exist(Player player) => await framework.Sql.ExistsAsync<CharacterData>("characters", x => x.RockstarId == player.Identifiers["license"]);

        public async Task Create(CharacterData data) => await framework.Sql.InsertOrUpdateAsync("characters", data);

        public async Task Save(Player player)
        {
            var rockstarId = player.Identifiers["license"];

            if (Characters.ContainsKey(rockstarId))
            {
                var character = Characters[rockstarId];
                await framework.Sql.InsertOrUpdateAsync("characters", character);
                framework.Logger.Info("[Character] Saved: " + rockstarId);
            }
        }

        #region Events

        [EventHandler("Character.SetPed")]
        protected void OnSetPedEvent(int player, uint model, int variation) => framework.Players[player].TriggerEvent("Character.SetPed", model, variation);

        [EventHandler("Character.Save")]
        protected async void OnSaveEvent([FromSource] Player player, string json)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json))
            {
                framework.Logger.Error("[Character] Unable to save character of player: " + player.Name + ", json contains an error.");
                return;
            }

            var character = JsonConvert.DeserializeObject<CharacterData>(json);

            if (Characters.ContainsKey(character.RockstarId))
            {
                Characters[character.RockstarId] = character;
            }
            else
            {
                Characters.Add(character.RockstarId, character);
                await Create(character);
            }

            await BaseScript.Delay(0);
        }

        protected async void PlayerDisconnecting(object sender, SDK.Server.Events.PlayerDisconnectingEventArgs e) => await Save(e.Player);

        [EventHandler("Character.SetMoney")]
        protected void OnSetMoneyEvent(int player, decimal amount) => framework.Players[player].TriggerEvent("Character.SetMoney", amount);

        [EventHandler("Character.SetBank")]
        protected void OnSetBankEvent(int player, decimal amount) => framework.Players[player].TriggerEvent("Character.SetBank", amount);

        [EventHandler("Character.AddMoney")]
        protected void OnAddMoneyEvent(int player, decimal amount) => framework.Players[player].TriggerEvent("Character.AddMoney", amount);

        [EventHandler("Character.AddBank")]
        protected void OnAddBankEvent(int player, decimal amount) => framework.Players[player].TriggerEvent("Character.AddBank", amount);

        [EventHandler("Character.RemoveMoney")]
        protected void OnRemoveMoneyEvent(int player, decimal amount) => framework.Players[player].TriggerEvent("Character.RemoveMoney", amount);

        [EventHandler("Character.RemoveBank")]
        protected void OnRemoveBankEvent(int player, decimal amount) => framework.Players[player].TriggerEvent("Character.RemoveBank", amount);

        #endregion
    }
}
