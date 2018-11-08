using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscRepoEF
{
    public class Repository<TEntity> where TEntity : class
    {
        private readonly IDesignTimeDbContextFactory<DbContext> contextFactory;
        private readonly EntityUpdater<TEntity> entityUpdater;
        private readonly Func<DbContext, DbSet<TEntity>> getDBSetFunc;

        public Repository(Func<DbContext, DbSet<TEntity>> getDBSetFunc, IDesignTimeDbContextFactory<DbContext> contextFactory)
        {
            this.getDBSetFunc = getDBSetFunc;
            this.contextFactory = contextFactory;
            entityUpdater = new EntityUpdater<TEntity>(contextFactory);
        }

        public void Add(TEntity entity)
        {
            try
            {
                TryToAdd(entity);
            }
            catch (ArgumentException e)
            {
                throw new DiscRepoEfException($"Object {entity} already exists in database.", e);
            }
            catch (SqlException e)
            {
                throw new DiscRepoEfException("Connection to database failed.", e);
            }
        }

        private void TryToAdd(TEntity entity)
        {
            ValidateEntityDoesntExistInDataBase(entity);
            entityUpdater.UpdateGraph(entity);
        }

        private void ValidateEntityDoesntExistInDataBase(TEntity entity)
        {
            using (DbContext context = contextFactory.CreateDbContext(new string[0]))
            {
                TEntity fromRepo = GetEntityFromRepo(context, entity);
                if (fromRepo != null) throw new DiscRepoEfException("Object already exists in database.");
            }
        }

        public void Delete(params object[] ids)
        {
            try
            {
                TryToDelete(ids);
            }
            catch (ArgumentException e)
            {
                throw new DiscRepoEfException($"Object with ids {GetKeysToString(ids)} not exists in database.", e);
            }
            catch (SqlException e)
            {
                throw new DiscRepoEfException("Connection to database failed.", e);
            }
        }

        private void TryToDelete(object[] ids)
        {
            using (DbContext context = contextFactory.CreateDbContext(new string[0]))
            {
                TEntity toDelete = context.Find<TEntity>(ids);
                if (toDelete == null)
                    throw new DiscRepoEfException($"Object of id {GetKeysToString(ids)} does not exists in database.");
                context.Entry(toDelete).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public TEntity Get(params object[] ids)
        {
            try
            {
                return TryToGet(ids);
            }
            catch (ArgumentException e)
            {
                throw new DiscRepoEfException($"Object with id {GetKeysToString(ids)} does not exists in database.", e);
            }
            catch (SqlException e)
            {
                throw new DiscRepoEfException("Connection to database failed.", e);
            }
        }

        private TEntity TryToGet(object[] ids)
        {
            using (DbContext context = contextFactory.CreateDbContext(new string[0]))
            {
                TEntity toReturn = Helper<TEntity>.GetEageredLoadedEntity(context, ids);
                if (toReturn == null) throw new DiscRepoEfException($"Object of id {GetKeysToString(ids)} does not exists in database.");

                return toReturn;
            }
        }

        private string GetKeysToString(object[] ids)
        {
            return string.Join("_", ids.Select(i => i.ToString()));
        }

        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                return TryToGetAll();
            }
            catch (SqlException e)
            {
                throw new DiscRepoEfException("Connection to database failed.", e);
            }
        }

        private IEnumerable<TEntity> TryToGetAll()
        {
            using (DbContext context = contextFactory.CreateDbContext(new string[0]))
            {
                // TODO: Find a better way to get all entities
                return Set(context).ToList().Select(e => context.Entry(e))
                    .Select(Helper<TEntity>.GetKeys)
                    .Select(e => Helper<TEntity>.GetEageredLoadedEntity(context, e.Keys.ToArray()))
                    .ToList();
            }
        }

        public void Update(TEntity entity)
        {
            try
            {
                TryToUpdate(entity);
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DiscRepoEfException($"Object {entity} does not exists in database.", e);
            }
            catch (SqlException e)
            {
                throw new DiscRepoEfException("Connection to database failed.", e);
            }
        }

        private void TryToUpdate(TEntity entity)
        {
            entityUpdater.UpdateGraph(entity);
        }

        private TEntity CreateEntity(TEntity id)
        {
            try
            {
                return Activator.CreateInstance<TEntity>();
            }
            catch (MissingMemberException e)
            {
                throw new DataException($"Class {id.GetType().Name} doesnt have a parameterless constructor", e);
            }
        }

        private TEntity GetEntityFromRepo(DbContext context, TEntity localEntity)
        {
            EntityEntry entry = context.Entry(localEntity);
            EntityKeys key = Helper<TEntity>.GetKeys(entry);
            return context.Find<TEntity>(key.Keys.ToArray());
        }

        private DbSet<TEntity> Set(DbContext context)
        {
            return getDBSetFunc.Invoke(context);
        }
    }
}