using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace DiscRepoEFTest
{
    public class LocalDbContextFactory : IDesignTimeDbContextFactory<Context>
    {
        private Guid guid;

        public LocalDbContextFactory()
        {
            guid = Guid.NewGuid();
        }

        public Context CreateDbContext(string[] args)
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=localhost; Database=DiscRepoEFTest;Trusted_Connection=True;")
                .Options;
            return new Context(options);
        }
    }
}