using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;

namespace Average.Server.Handlers
{
    internal class ClientHandler : IHandler
    {
        private readonly ClientService _clientService;
        private readonly CharacterService _characterService;
        private readonly CharacterCreatorService _characterCreatorService;
        private readonly CommandHandler _commandHandler;
        private readonly WorldService _worldService;

        public ClientHandler(CharacterCreatorService characterCreatorService, ClientService clientService, CharacterService characterService, CommandHandler commandHandler, WorldService worldService)
        {
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

            if (await _characterService.Exists(client))
            {
                // Spawn character

                Logger.Debug("Spawn character");

                _characterService.OnSpawnPed(client);
                _worldService.OnSetWorldForClient(client);
            }
            else
            {
                // Create character

                Logger.Debug("Create character");
                _characterCreatorService.StartCreator(client);
            }

            Logger.Debug("Client initialized: " + client.Name + ", " + client.ServerId);
        }
    }
}
