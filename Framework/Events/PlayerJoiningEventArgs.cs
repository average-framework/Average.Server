﻿using System;

namespace Average.Server.Framework.Events
{
    public class PlayerJoiningEventArgs : EventArgs
    {
        public string Source { get; }
        public string OldId { get; }

        public PlayerJoiningEventArgs(string source, string oldId)
        {
            Source = source;
            OldId = oldId;
        }
    }
}
