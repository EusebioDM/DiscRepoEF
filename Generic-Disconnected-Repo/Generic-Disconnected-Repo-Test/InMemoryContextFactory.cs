using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Generic_Disconnected_Repo_Test
{
    public class InMemoryContextFactory : IDesignTimeDbContextFactory<Context>
    {
        private Guid guid;

        public InMemoryContextFactory()
        {
            guid = Guid.NewGuid();
        }

        public Context CreateDbContext(string[] args)
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(guid.ToString())
                .EnableSensitiveDataLogging()
                //.UseLazyLoadingProxies()
                .Options;
            return new Context(options);
        }
    }
}