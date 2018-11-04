using Generic_Disconnected_Repo_Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace Generic_Disconnected_Repo_Test
{
    public class Context : DbContext
    {
        public DbSet<Encounter> Encounters { get; set; }

        public Context(DbContextOptions options) : base(options)
        {
            
        }
    }
}