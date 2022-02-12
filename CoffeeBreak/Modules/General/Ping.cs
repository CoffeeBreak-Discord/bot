using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("ping", "Ping!")]
    public async Task Ping()
    {
        await this.RespondAsync("Pong!");
    }
}
