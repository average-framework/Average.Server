using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Mongo;
using Average.Shared.DataModels;

namespace Average.Server.Repositories
{
    internal class BankRepository : DatabaseRepoBase<BankData>, IRepository
    {
        public override string CollectionName => "bank_accounts";

        public BankRepository(DatabaseContextFactory databaseContextFactory) : base(databaseContextFactory)
        {

        }
    }
}