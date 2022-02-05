using CoffeeBreak.ThirdParty;
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
        Logging.Info($"{ctx.User.Id}[{ctx.Guild.Id}] executing {compInfo.Name}", "Command");
        return Task.CompletedTask;
    }

    private Task ClientLog(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Info: Logging.Info(msg.Message, msg.Source); break;
            case LogSeverity.Warning: Logging.Warning(msg.Message, msg.Source); break;
            case LogSeverity.Error: Logging.Error(msg.Message, msg.Source); break;
        }
        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SlashCommandInfo cmdInfo, IInteractionContext ctx, IResult result)
    {
        Logging.Info($"{ctx.User.Id}[{ctx.Guild.Id}] executing /{cmdInfo.Name}", "SlashCommand");
        return Task.CompletedTask;
    }
}
