using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("version", "Print the bot version")]
    public async Task Version()
    {
        string Version = System.Environment.GetEnvironmentVariable("VERSION") ?? "Development";
        string CommitHash = System.Environment.GetEnvironmentVariable("COMMIT") ?? "Unknown";
        await this.RespondAsync($"Version: **{Version}**\nCommit Hash: **{CommitHash}**");
    }
}
