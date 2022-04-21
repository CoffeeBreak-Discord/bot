using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
[Group("poll", "Having some trouble to make decisions? Use this.")]
public partial class PollModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DiscordShardedClient _client;

    public enum PollStartMode
    {
        [ChoiceDisplay("Single Choice")] Single,
        [ChoiceDisplay("Multiple Choice")] Multiple
    }

    public PollModule(DiscordShardedClient client)
    {
        _client = client;
    }

    [SlashCommand("start", "Start the poll.")]
    public async Task StartCommandAsync(PollStartMode mode = PollStartMode.Single)
    {
        await this.RespondAsync("Start");
    }
}
