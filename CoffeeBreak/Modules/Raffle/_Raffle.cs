using CoffeeBreak.Models;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class RaffleModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DatabaseContext _db = new DatabaseContext();
    private DiscordShardedClient _client;
    public RaffleModule(DiscordShardedClient client)
    {
        _client = client;
    }
}
