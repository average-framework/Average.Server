using Average.Server.Framework.Database;
using Average.Server.Framework.Interfaces;
using Average.Shared.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Average.Server.Repositories
{
    public class CharacterRepository : DbRepoBase<CharacterData>, IRepository
    {
        public CharacterRepository(DbContextFactory factory) : base(factory)
        {

        }

        protected override IQueryable<CharacterData> SetWithIncludes(DatabaseContext context) =>
            context.Characters
                .Include(a => a.Economy)
                .Include(a => a.Core)
                .Include(a => a.Job)
                .Include(a => a.Skin)
                .Include(a => a.Outfit)
                .Include(a => a.Position);
    }
}