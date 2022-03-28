using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using StackExchange.Redis;

namespace CoffeeBreak.Modules;
[Group("stage", "Manage your stages")]
public partial class VoiceManagerStageModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DiscordShardedClient _client;
    private IConnectionMultiplexer _cache;

    public VoiceManagerStageModule(DiscordShardedClient client, IConnectionMultiplexer cache)
    {
        _client = client;
        _cache = cache;
    }

    // TODO: Make command
    // /stage join
    // /stage leave
    // /stage roles
    [RequireUserPermission(GuildPermission.ManageGuild)]
    [RequireUserPermission(GuildPermission.ManageEvents)]
    [RequireBotPermission(GuildPermission.MoveMembers)]
    [SlashCommand("join", "Make the bot watching the stage.", runMode: RunMode.Async)]
    public async Task JoinCommandAsync(
        [ChannelTypes(ChannelType.Stage), Summary(description: "Stage target")] IStageChannel? channel = null)
    {
        var userState = this.Context.User as IVoiceState;
        if (userState == null) return;
        channel = channel == null ? userState.VoiceChannel as IStageChannel : channel;

        if (channel == null)
        {
            await this.RespondAsync(
                "You didn't joined the stage. Please join again.",
                ephemeral: true);
            return;
        }

        await channel.ConnectAsync();
        await this.RespondAsync($"Joined **{channel.Name}** stage.", ephemeral: true);
    }

    [RequireUserPermission(GuildPermission.ManageGuild)]
    [RequireUserPermission(GuildPermission.ManageEvents)]
    [RequireBotPermission(GuildPermission.MoveMembers)]
    [SlashCommand("leave", "Make the bot watching the stage.", runMode: RunMode.Async)]
    public async Task LeaveCommandAsync()
    {
        var channel = this.Context.Guild.GetUser(_client.CurrentUser.Id).VoiceChannel as IStageChannel;
        if (channel == null)
        {
            await this.RespondAsync("This bot didn't joined any stage.", ephemeral: true);
            return;
        }

        await channel.DisconnectAsync();
        await this.RespondAsync($"Bot successfully disconnected from **{channel.Name}** stage.", ephemeral: true);
    }
}
