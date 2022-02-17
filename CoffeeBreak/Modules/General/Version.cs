using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("version", "Print the bot version")]
    public async Task Version()
    {
        string Version = System.Environment.GetEnvironmentVariable("VERSION") ?? "Development";
        string CommitSHA = System.Environment.GetEnvironmentVariable("COMMIT") ?? "Unknown";
        await this.RespondAsync($"Version: **{Version}**\nCommit SHA: **{CommitSHA}**");
    }
}
