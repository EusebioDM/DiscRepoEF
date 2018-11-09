using DiscRepoEFTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscRepoEFTest
{
    public class Context : DbContext
    {
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<Team> Teams { get; set; }

        public Context(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
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