using Average.Server.Framework.Database;
using Average.Server.Framework.Interfaces;
using Average.Shared.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Average.Server.Repositories
{
    public class UserRepository : DbRepoBase<UserData>, IRepository
    {
        public UserRepository(DbContextFactory factory) : base(factory)
        {

        }

        protected override IQueryable<UserData> SetWithIncludes(DatabaseContext context) =>
            context.Users.Include(a => a.Characters);
    }
}