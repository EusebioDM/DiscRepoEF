using System.Collections.Generic;

namespace Generic_Disconnected_Repo_Test.Entities
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public virtual ICollection<TeamUser> TeamUsers { get; set; }

        public override bool Equals(object obj)
        {
            return obj is User entity &&
                   UserName == entity.UserName;
        }

        public override int GetHashCode()
        {
            return 404878561 + EqualityComparer<string>.Default.GetHashCode(UserName);
        }
    }
}
