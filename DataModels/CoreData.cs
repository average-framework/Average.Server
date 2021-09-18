using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class CoreData : EntityBase
    {
        public long CharacterId { get; set; }
        public CharacterData Character { get; set; }

        public int Health { get; set; } = 100;
        public int Hunger { get; set; } = 100;
        public int Thirst { get; set; } = 100;
        public int Stamina { get; set; } = 100;
    }
}
