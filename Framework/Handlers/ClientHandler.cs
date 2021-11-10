using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Framework.Services;
using Average.Server.Scripts;
using System;

namespace Average.Server.Framework.Handlers
{
    internal class ClientHandler : IHandler
    {
        private readonly ClientService _clientService;
        private readonly CharacterService _characterService;
        private readonly CharacterCreatorScript _characterCreatorScript;
        private readonly WorldService _worldService;
        private readonly UIService _uiService;
        private readonly InventoryService _inventoryService;
        private readonly UserService _userService;

        public ClientHandler(UserService userService, InventoryService inventoryService, UIService uiService, CharacterCreatorScript characterCreatorScript, ClientService clientService, CharacterService characterService, WorldService worldService)
        {
            _userService = userService;
            _characterCreatorScript = characterCreatorScript;
            _clientService = clientService;
            _characterService = characterService;
            _worldService = worldService;
            _uiService = uiService;
            _inventoryService = inventoryService;
        }

        [ServerEvent("client:initialized")]
        private async void OnClientInitialized(Client client)
        {
            _clientService.AddClient(client);

            // UI Events
            _uiService.OnClientInitialized(client);

            // User
            _userService.OnClientInitialized(client);

            if (await _characterService.Exists(client))
            {
                // Spawn character

                Logger.Debug($"[ClientHandler] Spawn character for client: {client.Name}.");

                _characterService.OnSpawnPed(client);
                _worldService.OnClientInitialized(client);

                var userData = await _userService.Get(client);

                // Inventory
                _inventoryService.OnClientInitialized(client, userData.SelectedCharacterId);
            }
            else
            {
                // Create character

                Logger.Debug($"[ClientHandler] Creating character for client: {client.Name}.");
                _characterCreatorScript.StartCreator(client);
            }

            Logger.Write("Client", $"%{client.Name}({client.ServerId}) is initialized%", new Logger.TextColor(foreground: ConsoleColor.Green));
        }

        [ServerEvent("client:share_data")]
        private void OnShareData(Client client, string key, object value, bool @override)
        {
            _clientService.OnShareData(client, key, value, @override);
        }

        [ServerEvent("client:unshare_data")]
        private void OnUnshareData(Client client, string key)
        {
            _clientService.OnUnshareData(client, key);
        }
    }
}
