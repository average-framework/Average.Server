using Average.Server.Framework.Extensions;
using CitizenFX.Core;
using System;

namespace Average.Server.Framework.Model
{
    internal class Client
    {
        public Player Player { get; }
        public DateTime CreatedAt { get; }
        public int ServerId { get => int.Parse(Player.Handle); }
        public string License { get => Player.License(); }
        public string Name { get => Player.Name; }

        public Client(Player player)
        {
            Player = player;
            CreatedAt = DateTime.Now;
        }

        public void Kick(string reason)
        {
            Player.Drop(reason);
        }

        public void Emit(string eventName, params object[] args)
        {
            Player.TriggerEvent(eventName, args);
        }

        public void EmitLatent(string eventName, int bytesPerSecond, params object[] args)
        {
            Player.TriggerLatentEvent(eventName, bytesPerSecond, args);
        }
    }
}
