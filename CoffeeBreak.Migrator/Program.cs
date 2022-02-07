using CoffeeBreak.Migrator.MigrationList;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeBreak.Migrator;
class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = CreateServices();
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        switch (args[0])
        {
            case "--up": runner.MigrateUp(); break;
            case "--down": runner.MigrateDown(long.Parse(args[1] ?? "0")); break;
        }
    }

    private static IServiceProvider CreateServices()
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb.AddMySql5().WithGlobalConnectionString(GenerateConnectionString())
                    // Add migration list here
                    .ScanIn(typeof(_20220207_ClassicModeration).Assembly).For.Migrations()
                )
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    private static string GenerateConnectionString()
    {
        string server = Environment.GetEnvironmentVariable("MARIADB_HOST") ?? "localhost";
        string database = Environment.GetEnvironmentVariable("MARIADB_DATABASE") ?? "coffeebreak";
        string username = Environment.GetEnvironmentVariable("MARIADB_USERNAME") ?? "root";
        string password = Environment.GetEnvironmentVariable("MARIADB_PASSWORD") ?? "";
        return $"server={server};database={database};user={username};password={password}";
    }
}
