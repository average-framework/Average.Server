using Average.Server.Framework.Database;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Interfaces;
using Average.Shared.DataModels;
using CitizenFX.Core;
using Microsoft.EntityFrameworkCore;

namespace Average.Server.Repositories
{
    public class CharacterRepository : DbRepoBase<CharacterData>, IRepository
    {
        public CharacterRepository(DbContextFactory factory) : base(factory)
        {
            
        }

        public override async Task<CharacterData> Update(CharacterData entity)
        {
            //return base.Update(entity);

            try
            {
                using (var context = _dbContextFactory.CreateDbContext())
                {
                    var characterToRemove = await context.Characters
                        .Include(a => a.Economy)
                        .Include(a => a.Core)
                        .Include(a => a.Job)
                        .Include(a => a.Skin)
                        .Include(a => a.Outfit)
                        .Include(a => a.Position)
                        .FirstAsync(x => x.Id == entity.Id);

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        context.Characters.RemoveRange(new CharacterData[] { characterToRemove });
                        transaction.Commit();
                        await context.SaveChangesAsync();
                    }

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        context.Characters.Add(entity);
                        transaction.Commit();
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ex: " + ex.Message + "\n" + ex.InnerException);
            }

            return null;
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