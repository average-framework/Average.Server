using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class PositionData : EntityBase
    {
        public long CharacterId { get; set; }
        public CharacterData Character { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float H { get; set; }

        public PositionData()
        {

        }

        public PositionData(float x, float y, float z, float heading)
        {
            X = x;
            Y = y;
            Z = z;
            H = heading;
        }
    }
}
