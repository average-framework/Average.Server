using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Model;
using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Average.Server.Services
{
    internal class ClientService : IService
    {
        private readonly EventManager _eventManager;

        private int _clientIndexCount;

        public List<Client> Clients { get; } = new List<Client>();

        private PlayerList _players;

        public ClientService(EventManager eventManager, PlayerList players)
        {
            _eventManager = eventManager;
            _players = players;

            Logger.Write("ClientListService", "Initialized successfully");
        }

        public event EventHandler<ClientEventArgs> ClientAdded;
        public event EventHandler<ClientEventArgs> ClientRemoved;

        public Client this[Player player] => Get(player);
        public Client this[int clientIndex] => Get(clientIndex);

        internal Client Get(Player player)
        {
            return Clients.First(x => x.ServerId == int.Parse(player.Handle));
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

            _clientIndexCount++;
        }

        internal void RemoveClient(Client client)
        {
            OnClientRemoved(client);
            Clients.Remove(client);
        }

        internal void CleanupDuplicate(Player player)
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
            _eventManager.EmitLocalServer("client:added", new ClientEventArgs(client));
        }

        private void OnClientRemoved(Client client)
        {
            ClientRemoved?.Invoke(this, new ClientEventArgs(client));
            _eventManager.EmitLocalServer("client:removed", new ClientEventArgs(client));
        }

        public int ClientCount => Clients.Count;
        public int ClientIndexCount => _clientIndexCount;
    }
}
