using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using DryIoc;

namespace Average.Server.Handlers
{
    internal class ClientHandler : IHandler
    {
        private readonly ClientService _clientService;
        private readonly CharacterService _characterService;
        private readonly SpawnService _spawnService;
        private readonly UIService _uiService;
        private readonly CommandHandler _commandHandler;

        public ClientHandler(UIService uiService, ClientService clientService, CharacterService characterService, SpawnService spawnService, CommandHandler commandHandler)
        {
            _uiService = uiService;
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

            // Frames
            _uiService.LoadFrame(client, "menu");

            if (await _characterService.Exists(client))
            {
                // Spawn character

                //var character = await _characterService.Get(client);
                //_spawnService.OnSpawnCharacter(client, character);

            }
            else
            {
                // Create character

                //_spawnService.OnCreateCharacter(client);
            }
            
            Logger.Debug("Client initialized: " + client.Name + ", " + client.ServerId);
        }
    }
}
