using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DiscRepoEF
{
    internal static class Helper<TEntity> where TEntity : class
    {
        public static EntityKeys GetKeys(EntityEntry entry)
        {
            EntityKeys keys = new EntityKeys();
            foreach (PropertyEntry propety in entry.Properties)
                if (propety.Metadata.IsPrimaryKey())
                    keys.AddKey(propety.CurrentValue);
            return keys;
        }

        public static bool EntriesAreEqual(DbContext context, TEntity first, TEntity second)
        {
            EntityEntry firstEntry = context.Entry(first);
            EntityEntry secondEntry = context.Entry(second);

            return EntriesAreEqual(firstEntry, secondEntry);
        }

        public static bool EntriesAreEqual(EntityEntry first, EntityEntry second)
        {
            EntityKeys firstKey = GetKeys(first);
            EntityKeys secondKey = GetKeys(second);

            return firstKey.Equals(secondKey);
        }

        public static TEntity GetEageredLoadedEntity(DbContext context, params object[] keys)
        {
            TEntity entity = context.Find<TEntity>(keys);
            LoadAllPropetiesRecursively(context, entity);
            return entity;
        }

        public static TEntity LoadEntity(DbContext context, TEntity entity)
        {
            LoadAllPropetiesRecursively(context, entity);
            return entity;
        }

        private static void LoadAllPropetiesRecursively(DbContext context, object entity)
        {
            EntityEntry entry = context.Entry(entity);
            foreach (NavigationEntry navigation in entry.Navigations)
            {
                if (navigation.IsLoaded) continue;

                navigation.Load();
                dynamic value = navigation.CurrentValue;

                CallRecursivelyWithPropertyValue(context, navigation, value);
            }
        }

        private static void CallRecursivelyWithPropertyValue(DbContext context, NavigationEntry navigation, dynamic value)
        {
            if (navigation.Metadata.IsCollection())
                foreach (dynamic item in value)
                    LoadAllPropetiesRecursively(context, item);
            else
                LoadAllPropetiesRecursively(context, navigation.CurrentValue);
        }
    }
}