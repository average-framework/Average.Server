using Average.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Average.Server.Framework.Database
{
    internal abstract class DbCachedRepoBase<TEntity> where TEntity : class, IDbEntity, new()
    {
        private List<TEntity> _cachedEntities;
        protected readonly DbRepoBase<TEntity> _repo;

        public DbCachedRepoBase(DbRepoBase<TEntity> repository) => _repo = repository;

        public abstract void OnCachedDataReloaded();

        public abstract void OnCachedDataAdded(TEntity entity);

        public abstract void OnCachedDataRemoved(TEntity entity);

        public abstract void OnCachedDataUpdated(TEntity entity);

        protected void Initialize()
        {
            ReloadCachedData();
        }

        public virtual void ReloadCachedData()
        {
            _cachedEntities = _repo.GetAll(true);
            OnCachedDataReloaded();
        }

        public List<TEntity> GetAll() => _cachedEntities;

        public List<TEntity> GetAll(Func<TEntity, bool> predicate) => _cachedEntities.Where(predicate).ToList();

        public TEntity Get(long id) => Get(e => e.Id == id);

        public TEntity Get(Func<TEntity, bool> predicate) => _cachedEntities.FirstOrDefault(predicate);

        public Task<bool> Delete(long id)
        {
            var item = Get(id);

            if (item != null)
            {
                OnCachedDataRemoved(item);
                _cachedEntities.Remove(item);
            }

            return _repo.Delete(id);
        }


        public void Update(TEntity entity)
        {
            var cachedEntity = Get(entity.Id);
            _cachedEntities.Remove(cachedEntity);

            var newEntity = _repo.Update(entity);
            _cachedEntities.Add(newEntity.Result);

            OnCachedDataUpdated(newEntity.Result);
        }

        public virtual TEntity Add(TEntity entity)
        {
            var newEntity = _repo.Add(entity);
            var fullNewEntity = _repo.Get(newEntity.Id, true);

            _cachedEntities.Add(fullNewEntity);

            OnCachedDataAdded(fullNewEntity);
            return fullNewEntity;
        }

        public virtual bool Delete(TEntity entity)
        {
            _cachedEntities.Remove(entity);

            OnCachedDataRemoved(entity);
            return _repo.Delete(_repo.Get(entity.Id));
        }
    }
}
