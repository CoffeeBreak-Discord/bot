using CoffeeBreak.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class RaffleModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DatabaseService _db;
    private DiscordShardedClient _client;
    public RaffleModule(DatabaseService db, DiscordShardedClient client)
    {
        _db = db;
        _client = client;
    }
}
