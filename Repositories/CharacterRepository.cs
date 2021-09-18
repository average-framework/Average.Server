using Average.Server.DataModels;
using Average.Server.Framework.Database;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;

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
