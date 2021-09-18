using Average.Server.Framework.Database;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;

namespace Average.Server.Repositories
{
    public class CharacterRepository : DbRepoBase<CharacterData>, IRepository
    {
        public CharacterRepository(DbContextFactory factory) : base(factory)
        {
            Logger.Warn("Character repo");
        }

        //protected override IQueryable<UserData> SetWithIncludes(DatabaseContext context)
        //{
        //    return context.Accounts.Include(a => a.Characters);
        //}
    }
}
