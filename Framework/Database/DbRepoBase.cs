using Average.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
            var context = _dbContextFactory.CreateDbContext();

            context.Entry(entity).State = EntityState.Modified;

            await context.SaveChangesAsync();
            await context.DisposeAsync();

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

            await context.SaveChangesAsync();
            await context.DisposeAsync();

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
            context.SaveChanges();
            context.Dispose();

            return true;
        }
    }
}
