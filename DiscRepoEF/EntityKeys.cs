using System.Collections.Generic;
using System.Linq;

namespace DiscRepoEF
{
    internal class EntityKeys
    {
        private readonly ICollection<object> keys;

        public EntityKeys()
        {
            keys = new List<object>();
        }

        public EntityKeys(ICollection<object> keys)
        {
            this.keys = keys;
        }

        public IEnumerable<object> Keys => keys;

        public void AddKey(object key)
        {
            keys.Add(key);
        }

        public override bool Equals(object obj)
        {
            if (obj is EntityKeys other)
                return Keys.SequenceEqual(other.Keys);
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (object p in keys) hashCode ^= p.GetHashCode(); // XOR is used to prevent Integer Overflow

            return hashCode;
        }
    }
}