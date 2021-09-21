﻿using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.EventsArgs;
using Average.Server.Framework.Extensions;
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

        public List<Client> Clients { get; } = new List<Client>();

        private PlayerList _players;

        public ClientService(EventManager eventManager, PlayerList players)
        {
            _eventManager = eventManager;
            _players = players;

            Logger.Write("ClientService", "Initialized successfully");
        }

        public event EventHandler<ClientEventArgs> ClientAdded;
        public event EventHandler<ClientEventArgs> ClientRemoved;

        public Client this[Player player] => Get(player);

        internal Client Get(Player player)
        {
            return Clients.Find(x => x.Player.License() == player.License());
        }

        internal bool Exists(Client client)
        {
            return Clients.Contains(client);
        }

        internal void AddClient(Client client)
        {
            OnClientAdded(client);
            Clients.Add(client);
        }

        internal void RemoveClient(Client client)
        {
            OnClientRemoved(client);

            if(client == null)
            {
                Logger.Debug("Client is null");
                return;
            }

            Logger.Debug("Remove client info: " + client.Name);

            var c = Clients.Find(x => x.Player == client.Player);

            if(c == null)
            {
                Logger.Debug("C == null");
                return;
            }

            Clients.Remove(c);
        }

        internal void CleanupDuplicate(Player player)
        {
            Clients.RemoveAll(x => x.Player.Handle == player.Handle);
        }

        public void KickAll(string reason = "")
        {
            Clients.ForEach(x => x.Kick(reason));
            Clients.Clear();
        }

        private void OnClientAdded(Client client)
        {
            ClientAdded?.Invoke(this, new ClientEventArgs(client));
            _eventManager.Emit("client:added", new ClientEventArgs(client));
        }

        private void OnClientRemoved(Client client)
        {
            ClientRemoved?.Invoke(this, new ClientEventArgs(client));
            _eventManager.Emit("client:removed", new ClientEventArgs(client));
        }

        public int ClientCount => Clients.Count;
    }
}
