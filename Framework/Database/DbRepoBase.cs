using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Shared.DataModels;
using Average.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Average.Server.Framework.Database
{
    public abstract class DbRepoBase<TEntity> where TEntity : class, IDbEntity, new()
    {
        protected readonly DbContextFactory _dbContextFactory;

        public DbRepoBase(DbContextFactory dbContextFactory) => _dbContextFactory = dbContextFactory;

        protected virtual IQueryable<TEntity> SetWithIncludes(DatabaseContext context) => context.Set<TEntity>();

        public virtual List<TEntity> GetAll(bool includeChild = false)
        {
            var context = _dbContextFactory.CreateDbContext();
            var result = includeChild ? SetWithIncludes(context).ToList() : context.Set<TEntity>().ToList();

            context.Dispose();

            return result;
        }

        public virtual TEntity Get(long id, bool includeChildData = false) => Get(e => e.Id == id, includeChildData);

        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate, bool includeChildData = false)
        {
            var context = _dbContextFactory.CreateDbContext();
            var dataSet = includeChildData ? SetWithIncludes(context) : context.Set<TEntity>();
            var entity = dataSet.FirstOrDefault(predicate);

            context.Dispose();

            return entity;
        }

        public virtual async Task<TEntity> Update(TEntity entity)
        {
            try
            {
                using (var context = _dbContextFactory.CreateDbContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        
                        await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ex: " + ex.Message + "\n" + ex.InnerException);
            }

            //var context = _dbContextFactory.CreateDbContext();

            //context.Entry(entity).State = EntityState.Modified;

            ////context.ChangeTracker.DetectChanges();

            //await context.SaveChangesAsync();
            //await context.DisposeAsync();

            return Get(entity.Id, true);
        }

        public virtual async Task<TEntity> UpdateWithChilds(TEntity entity)
        {
            var context = _dbContextFactory.CreateDbContext();
            var tempEntity = entity;

            context.Entry(entity).State = EntityState.Modified;

            await Delete(entity.Id);
            await Add(tempEntity);

            return Get(entity.Id, true);
        }

        public virtual async Task<bool> Delete(long id)
        {
            var context = _dbContextFactory.CreateDbContext();
            var entity = Get(id);
            var boolResult = entity != null && Delete(entity);

            await context.DisposeAsync();

            return boolResult;
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            var context = _dbContextFactory.CreateDbContext();
            var createdEntity = context.Set<TEntity>().Add(entity).Entity;

            //await context.SaveChangesAsync();
            //await context.DisposeAsync();

            //using (var context = _dbContextFactory.CreateDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                await context.SaveChangesAsync();
                transaction.Commit();
            }

            return createdEntity;
        }

        public virtual int DeleteWhere(Expression<Func<TEntity, bool>> predicate)
        {
            var context = _dbContextFactory.CreateDbContext();
            var entities = context.Set<TEntity>().Where(predicate);

            entities.ForEachAsync(x => Delete(x));
            context.Dispose();

            return entities.Count();
        }

        public virtual bool Delete(TEntity entity)
        {
            var context = _dbContextFactory.CreateDbContext();

            context.Entry(entity).State = EntityState.Deleted;
            context.Set<TEntity>().Remove(entity);
            //context.SaveChanges();
            //context.Dispose();

            using (var transaction = context.Database.BeginTransaction())
            {
                context.SaveChanges();
                transaction.Commit();
            }

            return true;
        }
    }
}
