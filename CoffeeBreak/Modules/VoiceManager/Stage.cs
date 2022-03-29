using CoffeeBreak.Models;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CoffeeBreak.Modules;
[Group("stage", "Manage your stages")]
public partial class VoiceManagerStageModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DiscordShardedClient _client;
    private IConnectionMultiplexer _cache;
    private DatabaseContext _db = new DatabaseContext();
    private string _cacheKeys
    {
        get { return $"coffeebreak_stage:{this.Context.Guild.Id}"; }
    }

    public VoiceManagerStageModule(DiscordShardedClient client, IConnectionMultiplexer cache)
    {
        _client = client;
        _cache = cache;
    }

    [RequireUserPermission(GuildPermission.ManageGuild)]
    [RequireUserPermission(GuildPermission.ManageEvents)]
    [RequireBotPermission(GuildPermission.MoveMembers)]
    [SlashCommand("join", "Make the bot watching the stage.")]
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

        // Check if bot is connected
        var botState = this.Context.Guild.GetUser(_client.CurrentUser.Id).VoiceChannel as IStageChannel;
        if (botState != null)
        {
            await this.RespondAsync(
                $"The bot already joined in <#{channel.Id}> stage.",
                ephemeral: true);
            return;
        }
 
        // Check channel
        var data = await _db.StageConfig.FirstOrDefaultAsync(x => x.GuildID == this.Context.Guild.Id);
        if (data == null)
        {
            await this.RespondAsync(
                "You didn't set the Speaker Role. Please use `/stage role <role>` to set the Speaker Role.",
                ephemeral: true);
            return;
        }

        // Insert to cache pool
        if (await _cache.GetDatabase().KeyExistsAsync(_cacheKeys)) await _cache.GetDatabase().KeyDeleteAsync(_cacheKeys);
        await _cache.GetDatabase().StringSetAsync(_cacheKeys, data.RoleID);

        await channel.ConnectAsync();
        await this.RespondAsync($"Joined <#{channel.Id}> stage.", ephemeral: true);
    }

    [RequireUserPermission(GuildPermission.ManageGuild)]
    [RequireUserPermission(GuildPermission.ManageEvents)]
    [RequireBotPermission(GuildPermission.MoveMembers)]
    [SlashCommand("leave", "Make the bot leave the stage.", runMode: RunMode.Async)]
    public async Task LeaveCommandAsync()
    {
        var channel = this.Context.Guild.GetUser(_client.CurrentUser.Id).VoiceChannel as IStageChannel;
        if (channel == null)
        {
            await this.RespondAsync("This bot didn't joined any stage.", ephemeral: true);
            return;
        }

        if (await _cache.GetDatabase().KeyExistsAsync(_cacheKeys)) await _cache.GetDatabase().KeyDeleteAsync(_cacheKeys);
        await channel.DisconnectAsync();
        await this.RespondAsync($"Bot successfully disconnected from <#{channel.Id}> stage.", ephemeral: true);
    }

    [RequireUserPermission(GuildPermission.ManageGuild)]
    [RequireUserPermission(GuildPermission.ManageEvents)]
    [RequireBotPermission(GuildPermission.MoveMembers)]
    [SlashCommand("role", "Set speaker role in stage.")]
    public async Task RoleCommandAsync(
        [Summary(description: "Speaker's role name")] IRole role)
    {
        if (this.Context.Guild.GetRole(role.Id) == null)
        {
            await this.RespondAsync("Role not found.", ephemeral: true);
            return;
        }

        var data = await _db.StageConfig.FirstOrDefaultAsync(x => x.GuildID == this.Context.Guild.Id);
        if (data == null)
        {
            await _db.StageConfig.AddAsync(new StageConfig
            {
                GuildID = this.Context.Guild.Id,
                RoleID = role.Id
            });
        }
        else
        {
            data.RoleID = role.Id;
            _db.StageConfig.Update(data);
        }
        await _db.SaveChangesAsync();
        await this.RespondAsync($"<@&{role.Name}> successfully set as Speaker Role.", ephemeral: true);
    }
}
