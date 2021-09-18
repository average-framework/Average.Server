using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class EnterpriseData : EntityBase, IDbEntity
    {
        public string JobName { get; set; }
        public string TreasuryAmount { get; set; }
    }
}