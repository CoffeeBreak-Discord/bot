using CoffeeBreak.Function;
using CoffeeBreak.Models;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

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

    [RequireUserPermission(GuildPermission.ManageGuild)]
    [SlashCommand("stop", "Stop the poll")]
    public async Task StopCommandAsync(
        [Summary(description: "Poll ID - You can peek the ID from the bottom of poll")] ulong id)
        => await this.StopCancelAsync(id);

    [RequireUserPermission(GuildPermission.ManageGuild)]
    [SlashCommand("cancel", "Cancel the poll")]
    public async Task CancelCommandAsync(
        [Summary(description: "Poll ID - You can peek the ID from the bottom of poll")] ulong id)
        => await this.StopCancelAsync(id, true);

    private async Task StopCancelAsync(ulong id, bool isCanceled = false)
    {
        var data = await _db.PollRunning.FirstOrDefaultAsync(x => x.ID == id);
        bool isAdmin = this.Context.Guild.GetUser(this.Context.User.Id).Roles.Where(x => x.Permissions.Has(GuildPermission.Administrator)).Count() > 0;
        if (data == null)
        {
            await this.RespondAsync("ID not found. Please try again.", ephemeral: true);
            return;
        }
        if (this.Context.Guild.Id != data.GuildID)
        {
            await this.RespondAsync("ID not found in this server. Please try again.", ephemeral: true);
            return;
        }
        if (!isAdmin && this.Context.User.Id != data.UserID)
        {
            await this.RespondAsync("You cannot cancel that poll because you're not the creator or guild administrator", ephemeral: true);
            return;
        }
        if (data.IsExpired)
        {
            await this.RespondAsync("You cannot stop the expired poll.", ephemeral: true);
            return;
        }

        SocketTextChannel? channel = this.Context.Guild.GetTextChannel(data.ChannelID);
        if (channel == null) return;
        var message = await channel.GetMessageAsync(data.MessageID);
        if (message == null) return;

        Global.State.Poll.PollActive.Remove(id);
        await PollManager.StopPollAsync(this.Context.Guild, channel, message, _db, data.ID, isCanceled);
        await this.RespondAsync("Poll successfully " + (isCanceled ? "canceled!" : "stopped!"), ephemeral: true);
    }
}
