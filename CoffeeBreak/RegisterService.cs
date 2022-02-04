using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using CoffeeBreak.Services;

namespace CoffeeBreak;
public partial class Program
{
    private IServiceCollection SetCollector()
    {
        // Add your service here
        return new ServiceCollection()
            // Discord Shard Client
            .AddSingleton(new DiscordShardedClient(this._config))

            // Logging
            .AddSingleton<LoggingService>()

            // Interaction & Commands
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandlerService>();
    }

    private async void SetExtraStep(ServiceProvider build)
    {
        // If you have some extra step to initialize your service.
        // First of all, add your service in exlude and set the
        // "extra step" in here.

        // Initialize Command Handler async
        await build.GetRequiredService<CommandHandlerService>().InitializeAsync();
    }

    private ServiceProvider ConfigureServices()
    {
        var services = this.SetCollector();
        var build = services.BuildServiceProvider();
        this.SetExtraStep(build);

        foreach (var service in services.ToArray()) build.GetRequiredService(service.ServiceType);
        
        return build;
    }
}
