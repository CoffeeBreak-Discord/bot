using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("avatar", "Display user avatar")]
    public async Task Avatar(
        [Summary(description: "Target user that you want to stalk")] IUser? user = null)
    {
        user ??= this.Context.User;
        string Avatar = user.GetAvatarUrl(size: 512) ?? user.GetDefaultAvatarUrl();
        EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithAuthor(user)
            .WithTitle("Avatar URL")
            .WithUrl(Avatar)
            .WithImageUrl(Avatar);
        await this.RespondAsync(embed: embed.Build());
    }
}