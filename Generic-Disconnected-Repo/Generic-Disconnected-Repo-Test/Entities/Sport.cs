using System.Collections.Generic;

namespace Generic_Disconnected_Repo_Test.Entities
{
    public class Sport
    {
        public string SportName { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Sport entity &&
                   SportName == entity.SportName;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(SportName);
        }
    }
}
