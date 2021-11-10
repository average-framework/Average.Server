using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Mongo;
using Average.Shared.DataModels;

namespace Average.Server.Framework.Repositories
{
    internal class UserRepository : DatabaseRepoBase<UserData>, IRepository
    {
        public override string CollectionName => "users";

        public UserRepository(DatabaseContextFactory databaseContextFactory) : base(databaseContextFactory)
        {

        }
    }
}