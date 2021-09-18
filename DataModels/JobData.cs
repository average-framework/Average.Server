using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class JobData : EntityBase
    {
        public long CharacterId { get; set; }
        public CharacterData Character { get; set; }

        public string Name { get; set; } = "unemployed";
        public int Level { get; set; }
    }
}
