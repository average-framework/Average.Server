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
        private readonly SpawnService _spawnService;
        private readonly CommandHandler _commandHandler;

        public ClientHandler(CharacterCreatorService characterCreatorService, ClientService clientService, CharacterService characterService, SpawnService spawnService, CommandHandler commandHandler)
        {
            _characterCreatorService = characterCreatorService;
            _clientService = clientService;
            _characterService = characterService;
            _spawnService = spawnService;
            _commandHandler = commandHandler;
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

                Logger.Warn("Spawn character");

                //var character = await _characterService.Get(client);
                //_spawnService.OnSpawnCharacter(client, character);
            }
            else
            {
                // Create character

                Logger.Warn("Create character");
                _characterCreatorService.StartCreator(client);
                //_spawnService.OnCreateCharacter(client);
            }

            Logger.Debug("Client initialized: " + client.Name + ", " + client.ServerId);
        }
    }
}
