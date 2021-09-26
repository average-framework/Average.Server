using Average.Server.Framework.Database;
using Average.Server.Framework.Interfaces;
using Average.Shared.DataModels;

namespace Average.Server.Repositories
{
    public class WorldRepository : DbRepoBase<WorldData>, IRepository
    {
        public WorldRepository(DbContextFactory factory) : base(factory)
        {

        }

        //protected override IQueryable<WorldData> SetWithIncludes(DatabaseContext context)
        //{
        //    return context.Users.Include(a => a.Characters);
        //}
    }
}