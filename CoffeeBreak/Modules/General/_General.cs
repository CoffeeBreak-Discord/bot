using CoffeeBreak.Models;
using Discord.Interactions;
using Discord.WebSocket;
using StackExchange.Redis;

namespace CoffeeBreak.Modules;
public partial class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DatabaseContext _db = new DatabaseContext();
    private InteractionService _cmd;
    private DiscordShardedClient _client;
    private IConnectionMultiplexer _cache;
    public GeneralModule(DiscordShardedClient client, InteractionService cmd, IConnectionMultiplexer cache)
    {
        _cmd = cmd;
        _client = client;
        _cache = cache;
    }
}
