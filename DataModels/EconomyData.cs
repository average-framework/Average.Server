using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class EconomyData : EntityBase
    {
        public long CharacterId { get; set; }
        public CharacterData Character { get; set; }

        public decimal Money { get; set; }
        public decimal Bank { get; set; }

        public EconomyData()
        {

        }

        public EconomyData(decimal money, decimal bank)
        {
            Money = money;
            Bank = bank;
        }
    }
}
