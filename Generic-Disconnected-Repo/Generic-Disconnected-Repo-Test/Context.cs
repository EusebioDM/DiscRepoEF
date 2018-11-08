using Generic_Disconnected_Repo_Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace Generic_Disconnected_Repo_Test
{
    public class Context : DbContext
    {
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<Team> Teams { get; set; }

        public Context(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Sport>().HasKey(s => s.SportName);
            modelBuilder.Entity<Team>().HasKey(t => t.Name);
            modelBuilder.Entity<TeamUser>().HasKey("TeamName", "UserName");
            modelBuilder.Entity<User>().HasKey(u => u.UserName);
        }
    }
}