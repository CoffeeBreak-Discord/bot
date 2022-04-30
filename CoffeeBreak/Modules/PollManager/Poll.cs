using CoffeeBreak.Function;
using CoffeeBreak.Models;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
[Group("poll", "Manage giveaway on your server.")]
public partial class PollManagerModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DatabaseContext _db = new DatabaseContext();
    private DiscordShardedClient _client;

    public PollManagerModule(DiscordShardedClient client)
    {
        _client = client;
    }

    public enum PollChoiceMode
    {
        [ChoiceDisplay("Single Choice")] Single,
        [ChoiceDisplay("Multiple Choice")] Multiple
    }

    [RequireUserPermission(GuildPermission.ManageGuild)]
    [SlashCommand("create", "Create the poll")]
    public async Task StartCommandAsync(
        [Summary(description: "Kinds of choices")] PollChoiceMode mode = PollChoiceMode.Single)
    {
        switch (mode)
        {
            case PollChoiceMode.Single:
                await this.Context.Interaction.RespondWithModalAsync<PollManager.SingleChoiceModal>("modal_poll:single");
                break;
            case PollChoiceMode.Multiple:
                await this.Context.Interaction.RespondWithModalAsync<PollManager.MultipleChoiceModal>("modal_poll:multiple");
                break;
        }
    }
}
