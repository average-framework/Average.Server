using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using CitizenFX.Core;
using System;

namespace Average.Server.Framework.Model
{
    internal class Client
    {
        public Player Player { get; }
        public DateTime CreatedAt { get; }
        public string License { get => Player.License(); }
        public string Name { get => Player.Name; }
        public int ServerId { get => int.Parse(Player.Handle); }

        public Client(Player player)
        {
            Player = player;
            CreatedAt = DateTime.Now;
        }

        public void Kick(string reason)
        {
            Player.Drop(reason);
        }
    }
}
