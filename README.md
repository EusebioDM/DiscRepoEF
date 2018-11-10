# DiscRepoEF
Simple generic disconnected Entity Framework Core repository that resolves the entity graphs when adding or updating entities along with all its relations in a disconnected scenario.

## Quickstart
Below is a basic example of how to create a new repository and use it.
``` cs
    class Program
    {
        static void Main(string[] args)
        {
            Repository<Person> repository = new Repository<Person>(GetDbSet, new ContextFactory());

            Person person = new Person()
            {
                Name = "John",
                Age = 25
            };
            repository.Add(person);
            person.Age = 26;
            repository.Update(person);
        }

        private static DbSet<Person> GetDbSet(DbContext dbContext)
        {
            Context context = dbContext as Context;
            return context.Persons;
        }

        class ContextFactory : IDesignTimeDbContextFactory<Context>
        {
            public Context CreateDbContext(string[] args)
            {
                DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(@"Server=localhost; Database=DiscRepoEFTest;Trusted_Connection=True;")
                    .Options;
                return new Context(options);
            }
        }

        class Context : DbContext
        {
            public Context(DbContextOptions options) : base(options) { }

            public DbSet<Person> Persons { get; set; }
        }

        class Person
        {
            [Key]
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
```
If you want to add or update an entity without having to use the whole repository you can make use of the EntityUpdater class. Below is an example of that.
```cs
    class Program
    {
        static void Main(string[] args)
        {
            EntityUpdater<Person> entityUpdater = new EntityUpdater<Person>(new ContextFactory());

            Person person = new Person()
            {
                Name = "John",
                Age = 25
            };
            entityUpdater.AddOrUpdateGraph(person);
            person.Age = 26;
            entityUpdater.AddOrUpdateGraph(person);
        }

        class ContextFactory : IDesignTimeDbContextFactory<Context>
        {
            public Context CreateDbContext(string[] args)
            {
                DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(@"Server=localhost; Database=DiscRepoEFTest;Trusted_Connection=True;")
                    .Options;
                return new Context(options);
            }
        }

        class Context : DbContext
        {
            public Context(DbContextOptions options) : base(options) { }

            public DbSet<Person> Persons { get; set; }
        }

        class Person
        {
            [Key]
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
```

## Installing

Get the NuGet Package via the .NET CLI 
```
dotnet add package DiscRepoEF
```
or via the Package Manager
```
Install-Package DiscRepoEF
```

## Running the tests

To run the InLocalDBTests go to the ``` LocalDbContextFactory ``` class and adjust the connection string in the following line.
```
.UseSqlServer(@"Server=localhost; Database=DiscRepoEFTest;Trusted_Connection=True;")
```
There are no extra steps required to run the InMemoryTests


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

