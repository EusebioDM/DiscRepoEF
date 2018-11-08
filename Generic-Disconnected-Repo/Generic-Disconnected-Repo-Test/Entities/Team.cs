using System.Collections.Generic;

namespace Generic_Disconnected_Repo_Test.Entities
{
    public class Team
    {
        public string Name { get; set; }
        public string SportName { get; set; }
        public string Description { get; set; }
        public virtual Sport Sport { get; set; }

        protected bool Equals(Team other)
        {
            return string.Equals(Name, other.Name) && string.Equals(SportName, other.SportName) && string.Equals(Description, other.Description) && Equals(Sport, other.Sport);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Team) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SportName != null ? SportName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Sport != null ? Sport.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
