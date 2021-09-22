using CitizenFX.Core;
using System;

namespace Average.Server.Framework.Events
{
    public class PlayerDisconnectingEventArgs : EventArgs
    {
        public Player Player { get; }
        public string Reason { get; }

        public PlayerDisconnectingEventArgs(Player player, string reason)
        {
            Player = player;
            Reason = reason;
        }
    }
}
