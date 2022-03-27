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
}
