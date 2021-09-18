using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class FaceOverlayData : EntityBase
    {
        public long CharacterId { get; set; }
        public CharacterData Character { get; set; }

        public OverlayData Eyebrows { get; set; }
        public OverlayData Scars { get; set; }
        public OverlayData Eyeliners { get; set; }
        public OverlayData Lipsticks { get; set; }
        public OverlayData Acne { get; set; }
        public OverlayData Shadows { get; set; }
        public OverlayData Beardstabble { get; set; }
        public OverlayData Paintedmasks { get; set; }
        public OverlayData Ageing { get; set; }
        public OverlayData Blush { get; set; }
        public OverlayData Complex { get; set; }
        public OverlayData Disc { get; set; }
        public OverlayData Foundation { get; set; }
        public OverlayData Freckles { get; set; }
        public OverlayData Grime { get; set; }
        public OverlayData Hair { get; set; }
        public OverlayData Moles { get; set; }
        public OverlayData Spots { get; set; }
    }
}
