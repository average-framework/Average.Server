using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using CitizenFX.Core;
using System.Collections.ObjectModel;
using System.Linq;

namespace Average.Server.Services
{
    internal class ClientListService : IService
    {
        private int _clientCountIndex;

        public ObservableCollection<Client> Clients { get; private set; } = new ObservableCollection<Client>();

        public ClientListService()
        {
            Logger.Write("ClientListService", "Initialized successfully");
        }

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
            Clients.Add(client);
            _clientCountIndex++;
        }

        internal void RemoveClient(Client client)
        {
            Clients.Remove(client);
        }

        internal void RemoveAll(Player player)
        {
            var clients = Clients.Where(x => x.ServerId == int.Parse(player.Handle));

            foreach (var client in clients)
            {
                Clients.Remove(client);
            }
        }

        public void KickAll(string reason = "")
        {
            foreach (var client in Clients)
            {
                client.Kick(reason);
            }
        }

        public int ClientCount => Clients.Count;
    }
}
