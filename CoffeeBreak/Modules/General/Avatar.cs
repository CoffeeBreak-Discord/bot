using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("avatar", "Display user avatar")]
    public async Task AvatarCommandAsync(
        [Summary(description: "Target user that you want to stalk")] IUser? user = null)
    {
        user ??= this.Context.User;
        string Avatar = user.GetAvatarUrl(size: 512) ?? user.GetDefaultAvatarUrl();
        EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithAuthor(user)
            .WithTitle("Here's the avatar.")
            .WithUrl(Avatar)
            .WithImageUrl(Avatar);
        ComponentBuilder component = new ComponentBuilder()
            .WithButton(
                "Download Image",
                style: ButtonStyle.Link,
                url: user.GetAvatarUrl(size: 2048) ?? user.GetDefaultAvatarUrl());
        await this.RespondAsync(embed: embed.Build(), components: component.Build());
    }
}
