using Average.Server.Framework.Interfaces;
using Average.Server.Services;
using CitizenFX.Core;

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
    }
}
