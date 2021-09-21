using Average.Server.Enums;
using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.EventsArgs;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using CitizenFX.Core;

namespace Average.Server.Services
{
    internal class UserStateService : IService
    {
        private readonly UserService _userService;
        private readonly ClientService _clientService;

        public UserStateService(UserService userService, ClientService clientService)
        {
            _userService = userService;
            _clientService = clientService;

            Logger.Write("UserStateService", "Initialized successfully");
        }

        [ServerEvent(Events.PlayerConnecting)]
        internal async void PlayerConnecting(PlayerConnectingEventArgs e)
        {
            Logger.Debug("Connect 2: " + e.Player.Handle);
            Logger.Info($"[Server] Player connecting: {e.Player.Name} [{e.Player.License()}]");

            //Client client = null;

            _clientService.CleanupDuplicate(e.Player);

            e.Deferrals.defer();

            var userExist = await _userService.Exists(e.Player);

            if (!userExist)
            {
                _userService.Create(e.Player);
            }
            else
            {
                var userData = await _userService.Get(e.Player);
                _userService.UpdateLastConnectionTime(userData);

                if (userData.IsBanned)
                {
                    Logger.Info($"[Server] Player: {e.Player.Name} is banned.");

                    _userService.UpdateConnectionState(userData, false);
                    e.Deferrals.done("Vous êtes bannis du serveur.");
                    return;
                }

                if ((bool)Bootstrapper.BaseConfig["UseWhitelistSystem"])
                {
                    if (!userData.IsWhitelisted)
                    {
                        Logger.Info($"[Server] Player: {e.Player.Name} is not whitelisted.");

                        _userService.UpdateConnectionState(userData, false);
                        e.Deferrals.done("Vous n'êtes pas whitelist.");
                    }
                    else
                    {
                        //client = new Client(e.Player);
                        //_clientService.AddClient(client);
                        _userService.UpdateConnectionState(userData, true);

                        e.Deferrals.done();
                    }
                }
                else
                {
                    //client = new Client(e.Player);
                    //_clientService.AddClient(client);
                    _userService.UpdateConnectionState(userData, true);

                    e.Deferrals.done();
                }

                _userService.Update(userData);
            }

            //if(client != null)
            //{
            //    var lastHandle = e.Player.Handle;

            //    while (true)
            //    {
            //        var player = new PlayerList()[e.Player.Name];

            //        if (player != null)
            //        {
            //            if (lastHandle != player.Handle)
            //            {
            //                // The player server id has changed, need to set the new 
            //                client.SetServerId(int.Parse(player.Handle));
            //                Logger.Debug($"Reset client server id from {lastHandle} to {player.Handle}");
            //                lastHandle = player.Handle;
            //                break;
            //            }

            //            await BaseScript.Delay(250);
            //        }
            //    }
            //}
        }

        [ServerEvent(Events.PlayerDisconnecting)]
        internal async void PlayerDisconnecting(PlayerDisconnectingEventArgs e)
        {
            Logger.Debug("Disconnected 1: " + e.Player.Handle);

            var userData = await _userService.Get(e.Player);
            var client = _clientService.Get(e.Player);

            _clientService.RemoveClient(client);
            _clientService.CleanupDuplicate(e.Player);
            _userService.UpdateConnectionState(userData, false);
            _userService.Update(userData);
        }
    }
}
