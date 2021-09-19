using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Handlers;
using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Average.Server.Services
{
    internal class ClientListService : IService
    {
        private int _clientCountIndex;

        public List<Client> Clients { get; } = new List<Client>();

        public ClientListService()
        {
            Logger.Write("ClientListService", "Initialized successfully");
        }

        public event EventHandler<ClientEventArgs> ClientAdded;
        public event EventHandler<ClientEventArgs> ClientRemoved;

        public Client this[Player player] => Get(player);
        public Client this[int clientIndex] => Get(clientIndex);

        internal Client Get(Player player)
        {
            return Clients.FirstOrDefault(x => x.ServerId == int.Parse(player.Handle));
        }

        internal Client Get(int clientIndex)
        {
            return Clients.ElementAt(clientIndex);
        }

        internal bool Exists(Client client)
        {
            return Clients.Contains(client);
        }

        internal void AddClient(Client client)
        {
            OnClientAdded(client);
            Clients.Add(client);
            _clientCountIndex++;
        }

        internal void RemoveClient(Client client)
        {
            OnClientRemoved(client);
            Clients.Remove(client);
        }

        internal void RemoveAll(Player player)
        {
            Clients.RemoveAll(x => x.ServerId == int.Parse(player.Handle));
        }

        public void KickAll(string reason = "")
        {
            Clients.ForEach(x => x.Kick(reason));
            Clients.Clear();
        }

        private void OnClientAdded(Client client)
        {
            ClientAdded?.Invoke(this, new ClientEventArgs(client));
        }

        private void OnClientRemoved(Client client)
        {
            ClientRemoved?.Invoke(this, new ClientEventArgs(client));
        }

        public int ClientCount => Clients.Count;
    }
}
