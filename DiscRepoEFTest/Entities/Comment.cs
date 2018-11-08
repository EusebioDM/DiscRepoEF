using System;

namespace DiscRepoEFTest.Entities
{
    public class Comment
    {
        public virtual User User { get; set; }
        public string Message { get;  set; }
        public Guid Id { get; set; }

        protected bool Equals(Comment other)
        {
            return string.Equals(Message, other.Message) && Id.Equals(other.Id);
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
            unchecked
            {
                return ((Message != null ? Message.GetHashCode() : 0) * 397) ^ Id.GetHashCode();
            }
        }
    }
}
