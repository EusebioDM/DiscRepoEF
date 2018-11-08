using System.Collections.Generic;

namespace Generic_Disconnected_Repo_Test.Entities
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public virtual ICollection<TeamUser> TeamUsers { get; set; }

        protected bool Equals(User other)
        {
            return string.Equals(UserName, other.UserName) && string.Equals(Password, other.Password) && Equals(TeamUsers, other.TeamUsers);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TeamUsers != null ? TeamUsers.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
