using Average.Server.Enums;
using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using Average.Shared.Attributes;
using CitizenFX.Core;

namespace Average.Server.Handlers
{
    internal class ClientHandler : IHandler
    {
        private readonly ClientService _clientService;
        private readonly CommandHandler _commandHandler;
        private readonly PlayerList _players;

        public ClientHandler(ClientService clientService, CommandHandler commandHandler, PlayerList players)
        {
            _clientService = clientService;
            _commandHandler = commandHandler;
            _players = players;
        }

        [ServerEvent(Events.ResourceStart)]
        private async void OnResourceStart(ResourceStartEventArgs e)
        {
            if (e.Resource == "avg")
            {
                foreach (var player in _players)
                {
                    _clientService.AddClient(new Client(player));
                }

                Logger.Debug($"Client list updated. Added [{_clientService.Clients.Count}] clients.");
            }
        }

        [ClientEvent("client:game_initialized")]
        private void OnClientGameInitialized(Client client)
        {
            client.IsInitialized = true;
            _commandHandler.OnClientGameInitialized(client);

            Logger.Debug("Client initialized: " + client.Player.Name);
        }
    }
}
