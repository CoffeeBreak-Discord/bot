using CoffeeBreak.Function;
using Discord;
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

    [RequireUserPermission(GuildPermission.ManageGuild)]
    [SlashCommand("start", "Start the poll.")]
    public async Task StartCommandAsync(PollStartMode mode)
    {
        switch (mode)
        {
            case PollStartMode.Single:
                await this.Context.Interaction.RespondWithModalAsync<PollManager.PollSingleChoiceModal>("modal_poll:single");
                break;
            case PollStartMode.Multiple:
                await this.Context.Interaction.RespondWithModalAsync<PollManager.PollMultipleChoiceModal>("modal_poll:multiple");
                break;
        }
    }
}
