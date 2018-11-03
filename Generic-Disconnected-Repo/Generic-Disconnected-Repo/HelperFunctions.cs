using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Generic_Disconnected_Repo
{
    internal static class HelperFunctions<TEntity> where TEntity : class
    {
        public static EntityKeys GetKeys(EntityEntry entry)
        {
            EntityKeys keys = new EntityKeys();
            foreach (var propety in entry.Properties)
            {
                if (propety.Metadata.IsPrimaryKey())
                {
                    keys.AddKey(propety.CurrentValue);
                }
            }
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
    }
}
