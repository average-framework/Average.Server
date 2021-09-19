using CitizenFX.Core;
using System;

namespace Average.Server.Framework.Areas
{
    internal class InteractionArea
    {
        public Vector3 Position { get; private set; }
        public float Radius { get; private set; }
        public Action OnEnter { get; private set; }
        public Action OnLeave { get; private set; }
        public Func<bool> OnExecute { get; private set; }

        public InteractionArea()
        {
            
        }

        public InteractionArea(Vector3 position, float radius, Action onEnter = default, Action onLeave = default, Func<bool> onExecute = default)
        {
            Position = position;
            Radius = radius;
            OnEnter = onEnter;
            OnLeave = onLeave;
            OnExecute = onExecute;
        }
    }
}
