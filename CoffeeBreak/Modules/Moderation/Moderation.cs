using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class ModerationModule
{
    private DiscordShardedClient _client;

    public ModerationModule(DiscordShardedClient client)
    {
        _client = client;
    }
}
