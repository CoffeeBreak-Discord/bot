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
        //command.ComponentCommandExecuted += this.ComponentCommandExecuted;
        //command.ModalCommandExecuted += this.ModalCommandExecuted;
    }

    private Task ClientLog(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Info:
                Logging.Info(msg.Message, msg.Source);
                break;
            case LogSeverity.Warning:
                Logging.Warning($"{msg.Message}", msg.Source);
                break;
            case LogSeverity.Error:
                Logging.Error($"{msg.Exception.Message ?? msg.Message}\n{msg.Exception.StackTrace ?? ""}", msg.Source);
                break;
        }
        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SlashCommandInfo cmdInfo, IInteractionContext ctx, IResult result)
    {
        var execName = cmdInfo.Module.IsSlashGroup
            ? $"/{cmdInfo.Module.SlashGroupName} {cmdInfo.Name}"
            : $"/{cmdInfo.Name}";
        Logging.Info($"{ctx.User.Id}[{ctx.Guild.Id}] executing {execName}", "SlashCommand");

        if (result.Error != null)
            Logging.Error($"Something wrong when executing /{cmdInfo.Name}\n{result.ErrorReason}", "SlashCommand");

        return Task.CompletedTask;
    }

    private Task ComponentCommandExecuted(ComponentCommandInfo compInfo, IInteractionContext ctx, IResult result)
    {
        Logging.Info($"{ctx.User.Id}[{ctx.Guild.Id}] executing {compInfo.Name}", "CompCommand");

        if (result.Error != null)
            Logging.Error($"Something wrong when executing {compInfo.Name}\n{result.ErrorReason}", "CompCommand");

        return Task.CompletedTask;
    }

    private Task ModalCommandExecuted(ModalCommandInfo modalInfo, IInteractionContext ctx, IResult result)
    {
        Logging.Info($"{ctx.User.Id}[{ctx.Guild.Id}] executing modal {modalInfo.MethodName}.{modalInfo.Name}", "ModalCommand");

        if (result.Error != null)
            Logging.Info($"Something wrong when executing {modalInfo.MethodName}.{modalInfo.Name}\n{result.ErrorReason}", "ModalCommand");

        return Task.CompletedTask;
    }
}
