namespace Generic_Disconnected_Repo_Test.Entities
{
    public class TeamUser
    {
        public string TeamName { get; set; }
        public string SportName { get; set; }
        public virtual Team Team { get; set; }
        public virtual User User { get; set; }
        public string UserName { get; set; }

        public TeamUser()
        {
        }

        public TeamUser(Team team, User user)
        {
            Team = team;
            User = user;
            TeamName = team.Name;
            SportName = team.SportName;
            UserName = user.UserName;
        }

        protected bool Equals(TeamUser other)
        {
            return Equals(Team, other.Team) && Equals(User, other.User);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TeamUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Team != null ? Team.GetHashCode() : 0) * 397) ^ (User != null ? User.GetHashCode() : 0);
            }
        }
    }
}
