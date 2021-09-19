using Average.Server.Framework.Extensions;
using CitizenFX.Core;
using System;

namespace Average.Server.Framework.Model
{
    internal class Client
    {
        private readonly Player _player;

        public DateTime CreatedAt { get; private set; }
        public int ServerId { get => int.Parse(_player.Handle); }
        public int Ping { get => _player.Ping; }
        public string EndPoint { get => _player.EndPoint; }
        public Ped Character { get => _player.Character; }
        public string License { get => _player.License(); }
        public string Name { get => _player.Name; }
        public StateBag State { get => _player.State; }

        public Client(Player player)
        {
            _player = player;
            CreatedAt = DateTime.Now;
        }

        public void Kick(string reason)
        {
            _player.Drop(reason);
        }

        public void Emit(string eventName, params object[] args)
        {
            _player.TriggerEvent(eventName, args);
        }

        public void EmitLatent(string eventName, int bytesPerSecond, params object[] args)
        {
            _player.TriggerLatentEvent(eventName, bytesPerSecond, args);
        }
    }
}
