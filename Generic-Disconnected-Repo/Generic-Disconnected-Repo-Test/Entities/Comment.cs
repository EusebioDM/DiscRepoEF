using System;
using System.Collections.Generic;
using System.Text;

namespace Generic_Disconnected_Repo_Test.Entities
{
    public class Comment
    {
        public virtual User User { get; set; }
        public string Message { get;  set; }
        public Guid Id { get; set; }

        protected bool Equals(Comment other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Comment) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
