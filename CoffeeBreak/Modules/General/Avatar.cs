using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("avatar", "Display user avatar")]
    public async Task Avatar(
        [Summary(description: "Target user that you want to stalk")] IUser? user = null)
    {
        var ctxUser = user == null ? this.Context.User : user;
        string Avatar = ctxUser.GetAvatarUrl(size: 512) ?? ctxUser.GetDefaultAvatarUrl();
        EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithAuthor(ctxUser)
            .WithTitle("Avatar URL")
            .WithUrl(Avatar)
            .WithImageUrl(Avatar);
        await this.RespondAsync(embed: embed.Build());
    }
}