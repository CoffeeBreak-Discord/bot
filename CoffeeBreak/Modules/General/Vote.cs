using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("vote", "Vote the bot")]
    public async Task VoteCommandAsync()
    {
        var embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithTitle("Vote the bot!")
            .WithDescription("Not implemented")
            .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl())
            .Build();
        
        await this.RespondAsync(embed: embed, ephemeral: true);
    }
}
