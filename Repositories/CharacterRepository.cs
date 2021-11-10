using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Mongo;
using Average.Shared.DataModels;

namespace Average.Server.Repositories
{
    internal class CharacterRepository : DatabaseRepoBase<CharacterData>, IRepository
    {
        public override string CollectionName => "characters";

        public CharacterRepository(DatabaseContextFactory databaseContextFactory) : base(databaseContextFactory)
        {

        }
    }
}