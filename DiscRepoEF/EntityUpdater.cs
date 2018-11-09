using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscRepoEF
{
    /// <summary>
    /// Contains the methods needed to resolve the entity graphs that arises when working in a disconnected environment in Entity FrameWork.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to update/save.</typeparam>
    public class EntityUpdater<TEntity> where TEntity : class
    {
        private readonly IDesignTimeDbContextFactory<DbContext> contextFactory;
        private Queue<object> entitiesLeftToUpdate;
        private HashSet<EntityKeys> entitiesThatShouldBeInUpdate;

        public EntityUpdater(IDesignTimeDbContextFactory<DbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
            entitiesLeftToUpdate = new Queue<object>();
            entitiesThatShouldBeInUpdate = new HashSet<EntityKeys>();
        }

        /// <summary>
        /// Adds or updates the entity, including all of its related entities in the database, 
        /// </summary>
        /// <param name="entityToUpdate">The entity to be added or updated.</param>
        public void AddOrUpdateGraph(TEntity entityToUpdate)
        {
            entitiesLeftToUpdate = new Queue<object>();
            entitiesThatShouldBeInUpdate = new HashSet<EntityKeys>();
            entitiesLeftToUpdate.Enqueue(entityToUpdate);

            while (entitiesLeftToUpdate.Any()) UpdateRootEntityAndItsChildrenIfPossible();

            RemoveNoLongerPresentEntities(entityToUpdate);
        }

        private void UpdateRootEntityAndItsChildrenIfPossible()
        {
            object rootEntityToUpdate = entitiesLeftToUpdate.Peek();

            using (DbContext context = contextFactory.CreateDbContext(new string[0]))
            {
                TraverseEntityGraphUpdatingWhenPossible(rootEntityToUpdate, context);
                context.SaveChanges();
            }

            entitiesLeftToUpdate.Dequeue();
        }

        private void TraverseEntityGraphUpdatingWhenPossible(object rootEntityToUpdate, DbContext context)
        {
            Action<EntityEntryGraphNode> updateNodeRecursivelyAction = n => UpdateNodeRecursively(context, n);

            context.ChangeTracker.TrackGraph(rootEntityToUpdate, updateNodeRecursivelyAction); // Runs the updateNodeRecursivelyAction method on all nodes
        }

        private void UpdateNodeRecursively(DbContext context, EntityEntryGraphNode node)
        {
            EntityEntry current = node.Entry;
            EntityEntry fatherNode = node.SourceEntry;
            entitiesThatShouldBeInUpdate.Add(Helper<TEntity>.GetKeys(current));

            if (EntryExistsInChangeTracker(context, current)) // Entity is already being tracked in a different node so the current context cant track it
                EnqueueFatherNodeToLeftToUpdateQueue(fatherNode); // Entity will be updated in a new Conxtext in the future since it cant be tracked in the current context
            else
                SetEntityAsModifiedOrAdded(context, current, node);
        }

        private void SetEntityAsModifiedOrAdded(DbContext context, EntityEntry entry, EntityEntryGraphNode node)
        {
            if (EntryExistsInDb(context, entry))
                entry.State = EntityState.Modified;
            else
                entry.State = EntityState.Added;
        }

        private void EnqueueFatherNodeToLeftToUpdateQueue(EntityEntry fatherNode)
        {
            bool canEnqueueWithoutGettingStuckInLoop = !entitiesLeftToUpdate.Contains(fatherNode.Entity);
            if (canEnqueueWithoutGettingStuckInLoop) entitiesLeftToUpdate.Enqueue(fatherNode.Entity); // Entity is added to the queue so it can be added in a different context
        }

        private bool EntryExistsInDb(DbContext context, EntityEntry entry)
        {
            bool exists = entry.GetDatabaseValues() != null;
            return exists;
        }

        private bool EntryExistsInChangeTracker(DbContext context, EntityEntry entry)
        {
            IEnumerable<EntityEntry> entriesInChangeTracker = context.ChangeTracker.Entries();
            bool exists = entriesInChangeTracker.Any(e => Helper<TEntity>.EntriesAreEqual(e, entry));
            return exists;
        }


        private void RemoveNoLongerPresentEntities(TEntity entity)
        {
            using (DbContext context = contextFactory.CreateDbContext(new string[0]))
            {
                EntityEntry entry = context.Entry(entity);
                EntityKeys key = Helper<TEntity>.GetKeys(entry);
                TEntity root = context.Find<TEntity>(key.Keys.ToArray());
                RemoveEntitiesNotInUpdateRecusively(context, context.Entry(root), new HashSet<EntityKeys>());
                context.SaveChanges();
            }
        }

        private void RemoveEntitiesNotInUpdateRecusively(DbContext context, EntityEntry currentEntry, HashSet<EntityKeys> alreadyTraversed)
        {
            EntityKeys keys = Helper<TEntity>.GetKeys(currentEntry);
            bool haventTraversedThisEntity = !alreadyTraversed.Contains(keys);
            if (!haventTraversedThisEntity) return;
            
            alreadyTraversed.Add(keys);
            foreach (NavigationEntry property in currentEntry.Navigations)
            {
                property.Load();
                dynamic propertyCurrentValue = property.CurrentValue;
                if (property.Metadata.IsCollection())
                    RemoveEntitiesFromCollectionThatWereNotPartOftheUpdateAndCallRecursively(context, propertyCurrentValue, alreadyTraversed);
                else
                    CallThisMethodRecusivelyWithChildEntity(context, property, alreadyTraversed);
            }
        }

        private void CallThisMethodRecusivelyWithChildEntity(DbContext context, NavigationEntry property, HashSet<EntityKeys> alreadyTraversed)
        {
            EntityEntry entry = context.Entry(property.CurrentValue);
            RemoveEntitiesNotInUpdateRecusively(context, entry, alreadyTraversed);
        }

        private void RemoveEntitiesFromCollectionThatWereNotPartOftheUpdateAndCallRecursively(DbContext context, dynamic entitiesThatNeedToBeFiltered, HashSet<EntityKeys> alreadyTraversed)
        {
            List<dynamic> toBeDeleted = new List<dynamic>();
            foreach (dynamic entity in entitiesThatNeedToBeFiltered)
            {
                EntityEntry entry = context.Entry(entity);
                EntityKeys entityKey = Helper<TEntity>.GetKeys(entry);
                if (!entitiesThatShouldBeInUpdate.Contains(entityKey)) toBeDeleted.Add(entity);
                RemoveEntitiesNotInUpdateRecusively(context, entry, alreadyTraversed);
            }

            Action<dynamic> deleteFromCollectionOfEntities = e => entitiesThatNeedToBeFiltered.Remove(e);
            toBeDeleted.ForEach(deleteFromCollectionOfEntities);
        }
    }
}