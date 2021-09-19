using CitizenFX.Core;

namespace Average.Server.Framework.Model
{
    internal class NpcAreaBase
    {
        public uint Model { get; set; }
        public int Variation { get; set; }
        public float Scale { get; set; } = 1f;
        public float Stamina { get; set; } = 100f;
        public float MaxStamina { get; set; } = 100f;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int RoutingBucket { get; set; }
        public float Heading { get; set; }
        public float Health { get; set; } = 100f;
        public float MaxHealth { get; set; } = 100f;
        public int Alpha { get; set; } = 255;
        public bool HasTemporaryEventsBlocked { get; set; }
        public bool IsMissionEntity { get; set; }
        public bool IsDead { get; set; }
        public bool IsEntityFullyLooted { get; set; }
        public bool IsConsumedByFire { get; set; }
        public bool IsOnFire { get; set; }
        public bool IsFocus { get; set; }
        public bool IsDecalDisabled { get; set; }
        public bool IsDynamic { get; set; }
        public bool IsMotionBlurEnabled { get; set; }
        public bool HasGravity { get; set; } = true;
        public bool FreezePosition { get; set; }
        public bool CanPlayGestureAnims { get; set; } = true;
        public bool CanBeIncapacited { get; set; } = true;
        public bool CanBeDamaged { get; set; } = true;
        public bool CanBeTargetted { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public bool IsInvincible { get; set; }
    }
}
