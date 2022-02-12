using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class ModerationModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("kick", "Kick user")]
    public async Task Kick([Summary(description: "The person who want to kicked")] IUser user)
    {
        await this.RespondAsync($"You'll kick {user}");
    }
}
