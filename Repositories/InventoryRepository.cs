using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Mongo;
using Average.Shared.DataModels;

namespace Average.Server.Repositories
{
    internal class InventoryRepository : DatabaseRepoBase<StorageData>, IRepository
    {
        public override string CollectionName => "inventories";

        public InventoryRepository(DatabaseContextFactory databaseContextFactory) : base(databaseContextFactory)
        {

        }
    }
}