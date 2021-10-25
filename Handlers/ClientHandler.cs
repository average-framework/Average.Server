using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using System;

namespace Average.Server.Handlers
{
    internal class ClientHandler : IHandler
    {
        private readonly InputService _inputService;
        private readonly ClientService _clientService;
        private readonly CharacterService _characterService;
        private readonly CharacterCreatorService _characterCreatorService;
        private readonly CommandHandler _commandHandler;
        private readonly WorldService _worldService;
        private readonly GameService _gameService;
        private readonly UIService _uiService;
        private readonly InventoryService _inventoryService;
        private readonly UserService _userService;
        private readonly RayService _rayService;

        public ClientHandler(RayService rayService, UserService userService, InventoryService inventoryService, UIService uiService, GameService gameService, InputService inputService, CharacterCreatorService characterCreatorService, ClientService clientService, CharacterService characterService, CommandHandler commandHandler, WorldService worldService)
        {
            _rayService = rayService;
            _userService = userService;
            _gameService = gameService;
            _inputService = inputService;
            _characterCreatorService = characterCreatorService;
            _clientService = clientService;
            _characterService = characterService;
            _commandHandler = commandHandler;
            _worldService = worldService;
            _uiService = uiService;
            _inventoryService = inventoryService;
        }

        [ServerEvent("client:initialized")]
        private async void OnClientInitialized(Client client)
        {
            _clientService.AddClient(client);

            // Commands
            _commandHandler.OnClientInitialized(client);

            // UI Events
            _uiService.OnClientInitialized(client);

            // Inputs
            _inputService.OnRegisteringInputs(client);

            // Game
            _gameService.Init(client);

            // Ray
            _rayService.OnClientInitialized(client);

            if (await _characterService.Exists(client))
            {
                // Spawn character

                Logger.Debug($"[ClientHandler] Spawn character for client: {client.Name}.");

                _characterService.OnSpawnPed(client);
                _worldService.OnSetWorldForClient(client);

                var userData = await _userService.Get(client);

                // Inventory
                _inventoryService.OnClientInitialized(client, userData.SelectedCharacterId);
            }
            else
            {
                // Create character

                Logger.Debug($"[ClientHandler] Creating character for client: {client.Name}.");
                _characterCreatorService.StartCreator(client);
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
