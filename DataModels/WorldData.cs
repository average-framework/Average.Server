using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class WorldData : EntityBase, IDbEntity
    {
        public int WorldId { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public uint Weather { get; set; }
    }
}
