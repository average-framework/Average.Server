using CitizenFX.Core;

namespace Average.Server.Framework.Areas
{
    internal class BlipArea
    {
        public int Handle { get; set; }
        public float Scale { get; set; }
        public Vector3 Position { get; set; }
        public string Label { get; set; }
        public int Sprite { get; set; }
        public bool Display { get; set; }

        public BlipArea()
        {

        }

        public BlipArea(int sprite, string label, Vector3 position, float scale = 1f, bool display = true)
        {
            Sprite = sprite;
            Label = label;
            Position = position;
            Scale = scale;
            Display = display;
        }
    }
}
