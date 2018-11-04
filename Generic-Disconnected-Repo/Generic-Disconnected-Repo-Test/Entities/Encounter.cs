using System;
using System.Collections.Generic;

namespace Generic_Disconnected_Repo_Test.Entities
{
    public class Encounter
    {

        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public virtual Sport Sport { get; set; }
        public virtual Team HomeTeam { get; set; }
        public virtual Team AwayTeam { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public override bool Equals(object obj)
        {
            return obj is Encounter entity &&
                   Id.Equals(entity.Id);
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<Guid>.Default.GetHashCode(Id);
        }
    }
}
