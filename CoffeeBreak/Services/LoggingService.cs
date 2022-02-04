using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Services;
public class LoggingService
{
    public LoggingService(DiscordShardedClient client, InteractionService command)
    {
        client.Log += this.ClientLog;
        command.SlashCommandExecuted += this.SlashCommandExecuted;
        command.ComponentCommandExecuted += this.ComponentCommandExecuted;
    }

    private Task ComponentCommandExecuted(ComponentCommandInfo compInfo, IInteractionContext ctx, IResult result)
    {
        Console.WriteLine($"{ctx.User.Id} executing {compInfo.Name}");
        return Task.CompletedTask;
    }

    private Task ClientLog(LogMessage msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SlashCommandInfo cmdInfo, IInteractionContext ctx, IResult result)
    {
        Console.WriteLine($"{ctx.User.Id} executing /{cmdInfo.Name}");
        return Task.CompletedTask;
    }
}
