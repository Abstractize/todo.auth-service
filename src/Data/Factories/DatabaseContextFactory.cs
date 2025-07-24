using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Services.Common.Identity;

namespace Data.Factories;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    private const string MESSAGE = "Connection string is not provided. Please set the SQL_CONNECTION_STRING environment variable or pass it as an argument. Example: dotnet ef migrations add InitialCreate -- \"YourConnectionString\"";

    public DatabaseContext CreateDbContext(string[] args)
    {
        var serviceCollection = new ServiceCollection()
            .AddIdentityService();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

        string? SQL_CONNECTION_STRING = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])
            ? args[0]
            : Environment.GetEnvironmentVariable(nameof(SQL_CONNECTION_STRING));

        if (string.IsNullOrWhiteSpace(SQL_CONNECTION_STRING))
        {
            throw new InvalidOperationException(MESSAGE);
        }

        optionsBuilder.UseNpgsql(SQL_CONNECTION_STRING);

        return new DatabaseContext(serviceProvider.GetRequiredService<IIdentityService>(), optionsBuilder.Options);
    }
}