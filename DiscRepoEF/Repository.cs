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
    /// <summary>
    /// Generic entity repository for EntityFramework Core.
    /// Can performs the CRUD operations resolving the entity graphs that arises when working in a disconnected environment in Entity FrameWork.
    /// </summary>
    /// <typeparam name="TEntity"> Type that is saved in the repository, must be a reference type.</typeparam>
    public class Repository<TEntity> where TEntity : class
    {
        private readonly IDesignTimeDbContextFactory<DbContext> contextFactory;
        private readonly EntityUpdater<TEntity> entityUpdater;
        private readonly Func<DbContext, DbSet<TEntity>> dbSetGetter;

        /// <summary>
        /// Creates a new Repository of the given type.
        /// </summary>
        /// <param name="dbSetGetter">Function that returns the DbSet of the TEntity type from the context.</param>
        /// <param name="contextFactory">Factory that crates a new context.</param>
        public Repository(Func<DbContext, DbSet<TEntity>> dbSetGetter, IDesignTimeDbContextFactory<DbContext> contextFactory)
        {
            this.dbSetGetter = dbSetGetter;
            this.contextFactory = contextFactory;
            entityUpdater = new EntityUpdater<TEntity>(contextFactory);
        }
        
        /// <summary>
        /// Adds the entity to the repository.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        /// <exception cref="DiscRepoException">Throws when entity already exists in the repository.</exception>
        /// <exception cref="DiscRepoConnectionException">Throws when connection to the database failed.</exception>
        public void Add(TEntity entity)
        {
            try
            {
                TryToAdd(entity);
            }
            catch (ArgumentException e)
            {
                throw new DiscRepoException($"Object {entity} already exists in repository.", e);
            }
            catch (SqlException e)
            {
                throw new DiscRepoConnectionException("Connection to database failed.", e);
            }
        }

        private void TryToAdd(TEntity entity)
        {
            ValidateEntityDoesntExistInDataBase(entity);
            entityUpdater.AddOrUpdateGraph(entity);
        }

        private void ValidateEntityDoesntExistInDataBase(TEntity entity)
        {
            using (DbContext context = contextFactory.CreateDbContext(new string[0]))
            {
                TEntity fromRepo = GetEntityFromRepo(context, entity);
                if (fromRepo != null) throw new DiscRepoException("Object already exists in database.");
            }
        }
        
        /// <summary>
        /// Deletes item from repository. 
        /// </summary>
        /// <param name="keys">Key or keys if the entity has a composite key of the entity to delete.</param>
        /// <exception cref="DiscRepoException">Throws when entity with the given keys doesn't exists in the repository.</exception>
        /// <exception cref="DiscRepoConnectionException">Throws when connection to the database failed.</exception>
        public void Delete(params object[] keys)
        {
            try
            {
                TryToDelete(keys);
            }
            catch (ArgumentException e)
            {
                throw new DiscRepoException($"Object with ids {GetKeysToString(keys)} not exists in repository.", e);
            }
            catch (SqlException e)
            {
                throw new DiscRepoConnectionException("Connection to database failed.", e);
            }
        }

        private void TryToDelete(object[] keys)
        {
            using (DbContext context = contextFactory.CreateDbContext(new string[0]))
            {
                TEntity toDelete = context.Find<TEntity>(keys);
                if (toDelete == null)
                    throw new DiscRepoException($"Object of id {GetKeysToString(keys)} does not exists in database.");
                context.Entry(toDelete).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
        
        /// <summary>
        /// Returns the entity with the given keys.
        /// </summary>
        /// <param name="keys">They key or keys if the entity has a composite key of the entity to return.</param>
        /// <returns>The entity with the given keys</returns>
        /// <exception cref="DiscRepoException">Throws when entity with the given keys doesn't exists in the repository.</exception>
        /// <exception cref="DiscRepoConnectionException">Throws when connection to the database failed.</exception>
        public TEntity Get(params object[] keys)
        {
            try
            {
                return TryToGet(keys);
            }
            catch (ArgumentException e)
            {
                throw new DiscRepoException($"Object with id {GetKeysToString(keys)} does not exists in database.", e);
            }
            catch (SqlException e)
            {
                throw new DiscRepoConnectionException("Connection to database failed.", e);
            }
        }

        private TEntity TryToGet(object[] keys)
        {
            using (DbContext context = contextFactory.CreateDbContext(new string[0]))
            {
                TEntity toReturn = Helper<TEntity>.GetEageredLoadedEntity(context, keys);
                if (toReturn == null) throw new DiscRepoException($"Object of id {GetKeysToString(keys)} does not exists in database.");

                return toReturn;
            }
        }

        private string GetKeysToString(object[] ids)
        {
            return string.Join("_", ids.Select(i => i.ToString()));
        }
        
        /// <summary>
        /// Returns all entities in the repository.
        /// </summary>
        /// <returns>All the entities in the repository.</returns>
        /// <exception cref="DiscRepoConnectionException">Throws when the connection to the database failed.</exception>
        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                return TryToGetAll();
            }
            catch (SqlException e)
            {
                throw new DiscRepoConnectionException("Connection to database failed.", e);
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
        
        /// <summary>
        /// Updates the entity in the repository.
        /// </summary>
        /// <param name="entity">Entity with the updated values.</param>
        /// <exception cref="DiscRepoException">Throws when entity with the given keys doesn't exists in the repository.</exception>
        /// <exception cref="DiscRepoConnectionException">Throws when the connection to the database failed.</exception>
        public void Update(TEntity entity)
        {
            try
            {
                TryToUpdate(entity);
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DiscRepoException($"Object {entity} does not exists in database.", e);
            }
            catch (SqlException e)
            {
                throw new DiscRepoConnectionException("Connection to database failed.", e);
            }
        }

        private void TryToUpdate(TEntity entity)
        {
            entityUpdater.AddOrUpdateGraph(entity);
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
            return dbSetGetter.Invoke(context);
        }
    }
}