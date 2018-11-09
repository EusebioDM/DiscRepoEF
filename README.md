# DiscRepoEF
Simple generic disconnected Entity Framework Core repository that resolves the entity graphs when adding or updating entities in a disconnected scenario.


### Installing

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
