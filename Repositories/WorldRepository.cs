using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Mongo;
using Average.Shared.DataModels;

namespace Average.Server.Repositories
{
    internal class WorldRepository : DatabaseRepoBase<WorldData>, IRepository
    {
        public override string CollectionName => "worlds";

        public WorldRepository(DatabaseContextFactory databaseContextFactory) : base(databaseContextFactory)
        {

        }
    }
}