using CoffeeBreak.Migrator.MigrationList;
using CoffeeBreak.ThirdParty;
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
            .ConfigureRunner(rb => rb.AddMySql5().WithGlobalConnectionString(Database.GenerateConnectionString())
                    // Add migration list here
                    .ScanIn(typeof(_20220207_ClassicModeration).Assembly).For.Migrations()
                    .ScanIn(typeof(_20220302_Giveaway).Assembly).For.Migrations()
                    .ScanIn(typeof(_20220311_GiveawayParticipant).Assembly).For.Migrations()
                    .ScanIn(typeof(_20220328_StageConfig).Assembly).For.Migrations()
                )
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }
}
