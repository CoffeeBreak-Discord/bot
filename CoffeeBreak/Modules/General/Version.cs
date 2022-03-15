using Discord.Interactions;
using Discord;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("version", "Print the bot version")]
    public async Task Version()
    {
        EmbedBuilder embed = new EmbedBuilder()
            .WithAuthor(name: "Bot version", iconUrl: _client.CurrentUser.GetAvatarUrl())
            .AddField("Version", System.Environment.GetEnvironmentVariable("VERSION") ?? "Development", true)
            .AddField("Commit Hash", System.Environment.GetEnvironmentVariable("COMMIT") ?? "Unknown", true)
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithCurrentTimestamp()
            .WithFooter($"Requested by {this.Context.User}");
        await this.RespondAsync(embed: embed.Build());
    }
}
