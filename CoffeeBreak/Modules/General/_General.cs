using CoffeeBreak.Services;
using Discord.Interactions;
using Discord.WebSocket;
using StackExchange.Redis;

namespace CoffeeBreak.Modules;
public partial class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DatabaseService _db;
    private InteractionService _cmd;
    private DiscordShardedClient _client;
    private IConnectionMultiplexer _cache;
    public GeneralModule(DiscordShardedClient client, DatabaseService db, InteractionService cmd, IConnectionMultiplexer cache)
    {
        _db = db;
        _cmd = cmd;
        _client = client;
        _cache = cache;
    }
}
