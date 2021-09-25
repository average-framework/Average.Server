using Average.Server.Framework.Database;
using Average.Server.Framework.Diagnostics;
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
            Logger.Warn("Character repo");
        }

        protected override IQueryable<CharacterData> SetWithIncludes(DatabaseContext context)
        {
            return context.Characters;
            //return context.Characters.Include(a => a.Outfit);
        }
    }
}
