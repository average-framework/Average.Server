using Average.Server.Enums;
using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using CitizenFX.Core;
using System;

namespace Average.Server.Services
{
    internal class UserStateService : IService
    {
        private readonly UserService _userService;
        private readonly ClientListService _clientListService;

        private readonly TimeSpan _waitingTime = new TimeSpan(0, 0, 30);

        public UserStateService(UserService userService, ClientListService clientListService)
        {
            _userService = userService;
            _clientListService = clientListService;

            Logger.Write("UserStateService", "Initialized successfully");
        }

        [ServerEvent(Events.PlayerConnecting)]
        internal async void PlayerConnecting([FromSource] Player player, string kickReason, dynamic deferrals)
        {
            Logger.Info($"[Server] Player connecting: {player.Name}.");

            deferrals.defer();

            var userExist = await _userService.Exists(player);

            if (!userExist)
            {
                _userService.Create(player);
            }
            else
            {
                var userData = await _userService.Get(player);
                _userService.UpdateLastConnectionTime(userData);

                if (userData.IsBanned == 1)
                {
                    deferrals.done("Vous êtes bannis du serveur.");

                    _userService.UpdateConnectionState(userData, false);
                    Logger.Info($"[UserState] Player: {player.Name} is banned.");
                    return;
                }

                if ((bool)Main.BaseConfig["IsServerWhitelisted"])
                {
                    if (userData.IsWhitelisted == 0)
                    {
                        deferrals.done("Vous n'êtes pas whitelist.");

                        _userService.UpdateConnectionState(userData, false);
                        Logger.Info($"[UserState] Player: {player.Name} is not whitelisted.");
                    }
                    else
                    {
                        deferrals.done();

                        _clientListService.AddClient(new Client(player));
                        _userService.UpdateConnectionState(userData, true);
                    }
                }
                else
                {
                    deferrals.done();
                    _clientListService.AddClient(new Client(player));
                    _userService.UpdateConnectionState(userData, true);
                }

                _userService.Update(userData);
            }
        }

        [ServerEvent(Events.PlayerDisconnected)]
        internal async void PlayerDisconnecting([FromSource] Player player, string reason)
        {
            var userData = await _userService.Get(player);

            _clientListService.RemoveAll(player);
            _userService.UpdateConnectionState(userData, false);
            _userService.Update(userData);
        }
    }
}
