using Average.Server.Managers;
using CitizenFX.Core;
using SDK.Server.Interfaces;

namespace Average.Server.Handlers
{
    public class CharacterHandler : IHandler
    {
        private readonly CharacterService _characterService;
        private readonly PlayerList _players;

        public CharacterHandler(CharacterService service, PlayerList players)
        {
            _characterService = service;
            _players = players;

            #region Rpc

            //Rpc.Event("Character.Exist").On(async (message, callback) =>
            //{
            //    try
            //    {
            //        var player = Players[message.Target];
            //        var characterExist = await Exist(player);
            //        callback(characterExist.ToString().ToLower());

            //        Log.Debug($"[Character] Getted character exist: [{characterExist}]");
            //    }
            //    catch
            //    {
            //        Log.Debug("[Character] Unable to check if character exist.");
            //    }
            //});

            //Rpc.Event("Character.Load").On(async (message, callback) =>
            //{
            //    try
            //    {
            //        var player = Players[message.Target];
            //        var data = await Load(player.Identifiers["license"]);
            //        callback(data);

            //        Log.Debug($"[Character] Getted character: {data.License}.");
            //    }
            //    catch
            //    {
            //        Log.Debug("[Character] Unable get character.");
            //    }
            //});

            #endregion
        }

        #region Event

        private void OnSetPed(Player player, uint model, int variation) => player.TriggerEvent("character:set_ped", model, variation);

        private async void OnSetMoney(Player player, decimal amount)
        {
            var data = await _characterService.Get(player);
            data.Economy.Money = amount;
            _characterService.Update(data);
        }

        private async void OnSetBank(Player player, decimal amount)
        {
            var data = await _characterService.Get(player);
            data.Economy.Bank = amount;
            _characterService.Update(data);
        }

        private async void OnAddMoney(Player player, decimal amount)
        {
            var data = await _characterService.Get(player);
            data.Economy.Money += amount;
            _characterService.Update(data);
        }

        private async void OnAddBank(Player player, decimal amount)
        {
            var data = await _characterService.Get(player);
            data.Economy.Bank += amount;
            _characterService.Update(data);
        }

        private async void OnRemoveMoney(Player player, decimal amount)
        {
            var data = await _characterService.Get(player);
            data.Economy.Money -= amount;
            _characterService.Update(data);
        }

        private async void OnRemoveBank(Player player, decimal amount)
        {
            var data = await _characterService.Get(player);
            data.Economy.Bank -= amount;
            _characterService.Update(data);
        }

        #endregion
    }
}
