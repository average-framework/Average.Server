﻿using Average.Server.Enums;
using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
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

        [ServerEvent(ServerEvent.PlayerConnecting)]
        internal async void PlayerConnecting(PlayerConnectingEventArgs e)
        {
            Logger.Info($"[Server] Player connecting: {e.Player.Name} [{e.Player.License()}]");

            _clientService.CleanupDuplicate(e.Player);
            e.Deferrals.defer();

            var userExist = await _userService.Exists(e.Player);

            if (!userExist)
            {
                await _userService.Create(e.Player);
            }
            else
            {
                var userData = await _userService.Get(e.Player);
                _userService.UpdateLastConnectionTime(userData);

                if (userData.IsBanned)
                {
                    Logger.Info($"[Server] Player: {e.Player.Name} [{e.Player.License()}] is banned.");

                    _userService.UpdateConnectionState(userData, false);

                    await BaseScript.Delay(0);
                    e.Deferrals.done("Vous êtes bannis du serveur.");
                    return;
                }

                if ((bool)Bootstrapper.BaseConfig["IsServerWhitelisted"])
                {
                    if (!userData.IsWhitelisted)
                    {
                        Logger.Info($"[Server] Player: {e.Player.Name} [{e.Player.License()}] is not whitelisted.");

                        _userService.UpdateConnectionState(userData, false);

                        await BaseScript.Delay(0);
                        e.Deferrals.done("Vous n'êtes pas whitelist.");
                    }
                    else
                    {
                        _userService.UpdateConnectionState(userData, true);

                        await BaseScript.Delay(0);
                        e.Deferrals.done();
                    }
                }
                else
                {
                    _userService.UpdateConnectionState(userData, true);

                    await BaseScript.Delay(0);
                    e.Deferrals.done();
                }

                //await _userService.Update(userData);
            }
        }

        [ServerEvent(ServerEvent.PlayerDisconnecting)]
        internal async void PlayerDisconnecting(PlayerDisconnectingEventArgs e)
        {
            var player = e.Player;
            var userData = await _userService.Get(player);
            var client = _clientService.Get(player);

            _clientService.RemoveClient(client);
            _clientService.CleanupDuplicate(player);
            _userService.UpdateConnectionState(userData, false);

            await _userService.Update(userData);
        }
    }
}
