using Average.Server.Framework.Events;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;
using System;
using System.Collections.Specialized;

namespace Average.Server.Handlers
{
    internal class ClientListHandler : IHandler
    {
        private readonly ClientListService _clientListService;

        public ClientListHandler(ClientListService clientListService)
        {
            _clientListService = clientListService;

            _clientListService.Clients.CollectionChanged += Clients_CollectionChanged;
        }

        public event EventHandler<ClientEventArgs> ClientAdded;
        public event EventHandler<ClientEventArgs> ClientRemoved;

        private void Clients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var client in e.NewItems)
                    {
                        OnClientAdded(client as Client);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var client in e.OldItems)
                    {
                        OnClientRemove(client as Client);
                    }
                    break;
            }
        }

        private void OnClientAdded(Client client)
        {
            ClientAdded?.Invoke(this, new ClientEventArgs(client));
        }

        private void OnClientRemove(Client client)
        {
            ClientRemoved?.Invoke(this, new ClientEventArgs(client));
        }
    }
}
