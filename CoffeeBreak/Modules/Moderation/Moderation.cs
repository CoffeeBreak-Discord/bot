using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class ModerationModule
{
    private DiscordShardedClient _client;

    public ModerationModule(DiscordShardedClient client)
    {
        _client = client;
    }

    private SocketRole GetHighestRole(SocketGuildUser user)
    {
        return user.Roles.OrderByDescending(x => x.Position).ToArray()[0];
    }

    private bool IsExecutable(SocketGuildUser userContext, SocketGuildUser userTarget)
    {
        return this.GetHighestRole(userContext).Position > this.GetHighestRole(userTarget).Position;
    }
}