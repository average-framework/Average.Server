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
        private readonly CommandHandler _commandHandler;

        public ClientHandler(ClientService clientService, CommandHandler commandHandler)
        {
            _clientService = clientService;
            _commandHandler = commandHandler;
        }

        [ServerEvent("client:initialized")]
        private void OnClientInitialized(Client client)
        {
            _clientService.AddClient(client);
            _commandHandler.OnClientInitialized(client);
            
            Logger.Debug("Client initialized: " + client.Name + ", " + client.ServerId);
        }
    }
}
