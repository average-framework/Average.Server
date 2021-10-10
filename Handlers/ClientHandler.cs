﻿using Average.Server.Framework.Attributes;
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

        public ClientHandler(GameService gameService, InputService inputService, CharacterCreatorService characterCreatorService, ClientService clientService, CharacterService characterService, CommandHandler commandHandler, WorldService worldService)
        {
            _gameService = gameService;
            _inputService = inputService;
            _characterCreatorService = characterCreatorService;
            _clientService = clientService;
            _characterService = characterService;
            _commandHandler = commandHandler;
            _worldService = worldService;
        }

        [ServerEvent("client:initialized")]
        private async void OnClientInitialized(Client client)
        {
            _clientService.AddClient(client);

            // Commands
            _commandHandler.OnClientInitialized(client);

            // Inputs
            _inputService.OnRegisteringInputs(client);

            if (await _characterService.Exists(client))
            {
                // Spawn character

                Logger.Debug($"[ClientHandler] Spawn character for client: {client.Name}.");

                _characterService.OnSpawnPed(client);
                _worldService.OnSetWorldForClient(client);
            }
            else
            {
                // Create character

                Logger.Debug($"[ClientHandler] Creating character for client: {client.Name}.");
                _characterCreatorService.StartCreator(client);
            }

            _gameService.Init(client);

            Logger.Write("Client", $"%{client.Name}({client.ServerId}) is initialized%", new Logger.TextColor(foreground: ConsoleColor.Green));
        }
    }
}
