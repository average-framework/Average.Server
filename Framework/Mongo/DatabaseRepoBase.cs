using Average.Server.Framework.Diagnostics;
using Average.Shared.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Average.Server.Framework.Mongo
{
    internal abstract class DatabaseRepoBase<TEntity> where TEntity : class, IDatabaseEntity, new()
    {
        protected readonly DatabaseContextFactory _databaseContextFactory;

        public virtual string CollectionName { get; set; }

        public DatabaseRepoBase(DatabaseContextFactory databaseContextFactory) => _databaseContextFactory = databaseContextFactory;

        public FilterDefinitionBuilder<TEntity> Filter => Builders<TEntity>.Filter;
        public UpdateDefinitionBuilder<TEntity> Update => Builders<TEntity>.Update;

        internal IMongoCollection<TEntity> GetCollection()
        {
            return _databaseContextFactory.Database.GetCollection<TEntity>(CollectionName);
        }

        public virtual List<TEntity> GetAll()
        {
            var results = GetCollection().Find(Filter.Empty);
            return results.ToList();
        }

        public virtual List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter)
        {
            var results = GetCollection().Find(filter);
            return results.ToList();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            var results = await GetCollection().FindAsync<TEntity>(Filter.Empty);
            return results.ToList();
        }

        public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            var results = await GetCollection().FindAsync<TEntity>(filter);
            return results.ToList();
        }

        public virtual TEntity Get(string id)
        {
            var results = GetAll();
            return results.Find(x => x.Id == id);
        }

        public virtual TEntity Get(Func<TEntity, bool> predicate)
        {
            var results = GetAll();
            return results.FirstOrDefault(predicate);
        }

        public virtual TEntity GetBy(Expression<Func<TEntity, bool>> expression)
        {
            var results = GetCollection().Find(expression);
            return results.FirstOrDefault();
        }

        public virtual async Task<TEntity> GetByAsync(Expression<Func<TEntity, bool>> expression)
        {
            var results = await GetCollection().FindAsync(expression);
            return results.FirstOrDefault();
        }

        public virtual List<TEntity> GetAllBy(Expression<Func<TEntity, bool>> expression)
        {
            var results = GetCollection().Find(expression);
            return results.ToList();
        }

        public virtual async Task<List<TEntity>> GetAllByAsync(Expression<Func<TEntity, bool>> expression)
        {
            var results = await GetCollection().FindAsync(expression);
            return results.ToList();
        }

        public virtual async Task<TEntity> GetAsync(string id)
        {
            var results = await GetAllAsync();
            return results.Find(x => x.Id == id);
        }

        public virtual async Task<TEntity> GetAsync(Func<TEntity, bool> predicate)
        {
            var results = await GetAllAsync();
            return results.FirstOrDefault(predicate);
        }

        public virtual bool UpdateOne(Expression<Func<TEntity, bool>> expression, params UpdateDefinition<TEntity>[] definitions)
        {

            try
            {
                var results = GetCollection();

                foreach (var definition in definitions)
                {
                    results.UpdateOne(expression, definition);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> UpdateOneAsync(Expression<Func<TEntity, bool>> expression, params UpdateDefinition<TEntity>[] definitions)
        {
            try
            {
                var results = GetCollection();

                foreach (var definition in definitions)
                {
                    await results.UpdateOneAsync(expression, definition);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool ReplaceOne<TField>(Expression<Func<TEntity, TField>> field, TField value, TEntity newEntity)
        {
            var results = GetCollection();
            var result = results.ReplaceOne(Eq(field, value), newEntity);

            return result.ModifiedCount > 0;
        }

        public virtual async Task<bool> ReplaceOneAsync<TField>(Expression<Func<TEntity, TField>> field, TField value, TEntity newEntity)
        {
            var results = GetCollection();
            var result = await results.ReplaceOneAsync(Eq(field, value), newEntity);

            return result.ModifiedCount > 0;
        }

        public virtual bool Add(TEntity entity)
        {
            try
            {
                GetCollection().InsertOne(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            try
            {
                await GetCollection().InsertOneAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Ex: " + ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }

        public virtual bool DeleteOne(Expression<Func<TEntity, bool>> condition)
        {
            var result = GetCollection().DeleteOne(condition);
            return result.DeletedCount > 0;
        }

        public virtual async Task<bool> DeleteOneAsync(Expression<Func<TEntity, bool>> condition)
        {
            var result = await GetCollection().DeleteOneAsync(condition);
            return result.DeletedCount > 0;
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> condition)
        {
            return GetCollection().Find(condition).Any();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> condition)
        {
            return await GetCollection().Find(condition).AnyAsync();
        }

        public UpdateDefinition<TEntity> USet<TField>(Expression<Func<TEntity, TField>> field, TField value)
        {
            return Update.Set(field, value);
        }

        public FilterDefinition<TEntity> Eq<TField>(Expression<Func<TEntity, TField>> field, TField value)
        {
            return Filter.Eq(field, value);
        }
    }
}
