using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic_Disconnected_Repo_Test.Entities
{
    public class Encounter
    {

        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public virtual Sport Sport{ get; set; }
        public virtual Team HomeTeam { get; set; }
        public virtual Team AwayTeam { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();


        protected bool Equals(Encounter other)
        {
            return Id.Equals(other.Id) && DateTime.Equals(other.DateTime) &&
                   Equals(Sport, other.Sport) && Equals(HomeTeam, other.HomeTeam) &&
                   Equals(AwayTeam, other.AwayTeam) && Comments.ToHashSet().SetEquals(other.Comments.ToHashSet());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Encounter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ DateTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (Sport != null ? Sport.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (HomeTeam != null ? HomeTeam.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AwayTeam != null ? AwayTeam.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Comments != null ? Comments.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
