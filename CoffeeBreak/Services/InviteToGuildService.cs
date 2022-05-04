using CoffeeBreak.ThirdParty;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Services;
public class InviteToGuildService
{
    private DiscordShardedClient _client;
    private InteractionService _commands;

    public InviteToGuildService(DiscordShardedClient client, InteractionService commands)
    {
        _client = client;
        _commands = commands;
        client.JoinedGuild += this.JoinedGuild;
    }

    public async Task JoinedGuild(SocketGuild guild)
    {
        string desc = @"This bot using 100% interaction command. Just type `/` and check all the command that we have.
        
        This bot is a tool that can handling Voice Stage and Giveaway. For now we're still developing this bot. if you want to check the roadmap, just [click here](https://github.com/CoffeeBreak-Discord/roadmap).
        
        *Also, don't forget to check `/help` to see all the command on this bot.*";
        var embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithFooter($"(C) 2022-{DateTime.Now.Year} Coffee Break", _client.CurrentUser.GetAvatarUrl())
            .WithCurrentTimestamp()
            .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl())
            .WithImageUrl("https://cdn.discordapp.com/attachments/946050537814655046/961274373916987462/2022-04-06_22-30-02-1.gif")
            .WithTitle("Welcome to Coffee Break!")
            .WithDescription(desc)
            .Build();

        // If the default channel is protected, dont send anymore.
        try
        {
            await guild.DefaultChannel.SendMessageAsync(embed: embed);
            Logging.Info($"Sending welcome message to {guild.Id}.", "InviteGuild");
        }
        catch
        {
            Logging.Info($"Missing permission for sending welcome message to {guild.Id}, skipping.", "InviteGuild");
        }

        // Register command to server
        await _commands.RegisterCommandsToGuildAsync(guild.Id);
        Logging.Info($"Inject command to {guild.Id}.", "InviteGuild");
    }
}
