﻿using CitizenFX.Core;

namespace SDK.Server.Models
{
    public class Door
    {
        public Vector3 Position { get; set; }
        public bool IsLocked { get; set; }
        public float Range { get; set; }
        public string JobName { get; set; }
        public bool CanForce { get; set; } = true;
        public bool CanLockpick { get; set; } = true;
        public bool IsProperty { get; set; }
        public int PropertyId { get; set; } = -1;

        public Door(Vector3 position, bool isLocked, float range, string jobName)
        {
            Position = position;
            IsLocked = isLocked;
            Range = range;
            JobName = jobName;
        }
    }
}
