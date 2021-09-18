using Average.Server.Database;
using SDK.Server.Diagnostics;
using SDK.Server.Interfaces;
using SDK.Shared.DataModels;

namespace Average.Server.Repositories
{
    public class UserRepository : DbRepoBase<UserData>, IRepository
    {
        public UserRepository(DbContextFactory factory) : base(factory)
        {
            Logger.Warn("User repo");
        }

        //protected override IQueryable<UserData> SetWithIncludes(DatabaseContext context)
        //{
        //    return context.Accounts.Include(a => a.Characters);
        //}
    }
}
