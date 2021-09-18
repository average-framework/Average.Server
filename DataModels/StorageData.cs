using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using System.Collections.Generic;

namespace Average.Server.DataModels
{
    public class StorageData : EntityBase, IDbEntity
    {
        public string StorageId { get; set; }
        public double MaxWeight { get; set; }
        public List<StorageItemData> Items { get; set; } = new List<StorageItemData>();
    }
}
