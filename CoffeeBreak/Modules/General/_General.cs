using CoffeeBreak.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DatabaseService _db;
    private InteractionService _cmd;
    private DiscordShardedClient _client;
    public GeneralModule(DiscordShardedClient client, DatabaseService db, InteractionService cmd)
    {
        _db = db;
        _cmd = cmd;
        _client = client;
    }
}
