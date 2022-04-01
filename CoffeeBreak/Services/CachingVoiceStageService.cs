using Discord;
using Discord.WebSocket;
using StackExchange.Redis;

namespace CoffeeBreak.Services;
public class CachingVoiceStageService
{
    private DiscordShardedClient _client;
    private IConnectionMultiplexer _cache;

    public CachingVoiceStageService(DiscordShardedClient client, IConnectionMultiplexer cache)
    {
        _client = client;
        _cache = cache;

        client.UserVoiceStateUpdated += this.UserVoiceStateUpdated;
    }

    private async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState stateBefore, SocketVoiceState stateAfter)
    {
        var guildUser = user as SocketGuildUser;
        if (guildUser == null) return;
        var voiceState = guildUser.VoiceChannel as IStageChannel;
        Console.WriteLine(guildUser.VoiceChannel.GetChannelType());
        if (voiceState == null) return;

        // Ignore if bot didn't join the stage same as user
        var clientVoiceState = guildUser.Guild.GetUser(_client.CurrentUser.Id).VoiceChannel as IStageChannel;
        if (clientVoiceState == null) return;
        if (clientVoiceState.Name != voiceState.Name) return;

        // Get role from cache
        var getRoleID = await _cache.GetDatabase().StringGetAsync($"coffeebreak_stage:{guildUser.Guild.Id}");
        if (getRoleID.IsNullOrEmpty) return;
        ulong roleID = ulong.Parse(getRoleID);

        Console.WriteLine(user);
    }
}
