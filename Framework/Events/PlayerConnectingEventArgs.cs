using CitizenFX.Core;
using System;

namespace Average.Server.Framework.Events
{
    public class PlayerConnectingEventArgs : EventArgs
    {
        public Player Player { get; }
        public dynamic Kick { get; }
        public dynamic Deferrals { get; }

        public PlayerConnectingEventArgs(Player player, dynamic kick, dynamic deferrals)
        {
            Player = player;
            Kick = kick;
            Deferrals = deferrals;
        }
    }
}
