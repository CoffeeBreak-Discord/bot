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
        Single, Multiple
    }


    [RequireUserPermission(GuildPermission.ManageGuild)]
    [SlashCommand("create", "Create the poll")]
    public async Task StartCommandAsync(PollChoiceMode mode = PollChoiceMode.Single)
    {
        await this.Context.Interaction.RespondWithModalAsync<PollManager.SingleChoiceModal>("modal_poll:single");
    }
}
