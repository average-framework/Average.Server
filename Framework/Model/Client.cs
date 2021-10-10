using Average.Server.Framework.Extensions;
using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace Average.Server.Framework.Model
{
    internal class Client
    {
        public Player Player { get; }
        public DateTime CreatedAt { get; }
        public string License { get => Player.License(); }
        public string Name { get; }
        public int ServerId { get => int.Parse(Player.Handle); }
        public Dictionary<string, object> TempData { get; } = new Dictionary<string, object>();

        public Client(Player player)
        {
            Player = player;
            Name = player.Name;
            CreatedAt = DateTime.Now;
        }

        public void Kick(string reason = "")
        {
            Player.Drop(reason);
        }

        public static implicit operator Player(Client client) => client.Player;
    }
}
