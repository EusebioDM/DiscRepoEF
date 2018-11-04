using System.Collections.Generic;

namespace Generic_Disconnected_Repo_Test.Entities
{
    public class Team
    {
        public string Name { get; set; }
        public string SportName { get; set; }
        public string Description { get; set; }
        public virtual Sport Sport { get; set; }


        public override bool Equals(object obj)
        {
            return obj is Team other &&
                   Name == other.Name &&
                   SportName == other.SportName;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }


    }
}
