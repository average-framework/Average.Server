using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class TextureData : EntityBase
    {
        public long CharacterId { get; set; }
        public CharacterData Character { get; set; }

        public uint Albedo { get; set; }
        public uint Normal { get; set; }
        public uint Material { get; set; }
    }
}
