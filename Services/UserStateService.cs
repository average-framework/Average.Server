using Average.Server.Enums;
using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Extensions;
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
        private readonly DiscordService _discordService;

        private readonly TimeSpan _waitingTime = new TimeSpan(0, 0, 30);

        public UserStateService(UserService userService, ClientListService clientListService, DiscordService discordService)
        {
            _userService = userService;
            _clientListService = clientListService;
            _discordService = discordService;

            Logger.Write("UserStateService", "Initialized successfully");
        }

        [ServerEvent(Events.PlayerConnecting)]
        internal async void PlayerConnecting(PlayerConnectingEventArgs e)
        {
            Logger.Info($"[Server] Player connecting: {e.Player.Name} [{e.Player.License()}]");

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
                    Logger.Info($"[UserState] Player: {e.Player.Name} is banned.");

                    _userService.UpdateConnectionState(userData, false);
                    e.Deferrals.done("Vous êtes bannis du serveur.");
                    return;
                }

                if ((bool)Bootstrapper.BaseConfig["UseWhitelistSystem"])
                {
                    if (!userData.IsWhitelisted)
                    {
                        Logger.Info($"[UserState] Player: {e.Player.Name} is not whitelisted.");

                        _userService.UpdateConnectionState(userData, false);
                        e.Deferrals.done("Vous n'êtes pas whitelist.");
                    }
                    else
                    {
                        _clientListService.AddClient(new Client(e.Player));
                        _userService.UpdateConnectionState(userData, true);

                        e.Deferrals.done();
                    }
                }
                else
                {
                    _clientListService.AddClient(new Client(e.Player));
                    _userService.UpdateConnectionState(userData, true);

                    e.Deferrals.done();
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
