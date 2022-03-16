using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using CoffeeBreak.Function;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    public enum ChoiceUserInfo
    {
        Normal, Roles, Subscription
    }

    [SlashCommand("userinfo", "Display user info")]
    public async Task UserInfo(
        [Summary(description: "Target user that you want to stalk")] IUser? user = null,
        [Summary(description: "User info menu")] ChoiceUserInfo menu = ChoiceUserInfo.Normal)
    {
        var ctxUser = user == null ? this.Context.User : user;
        EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithAuthor(ctxUser.ToString())
            .WithThumbnailUrl(ctxUser.GetAvatarUrl())
            .WithCurrentTimestamp()
            .WithFooter($"Requested by {this.Context.User}");

        switch (menu)
        {
            case ChoiceUserInfo.Normal: this.GetNormalUserInfo(ref embed, ctxUser); break;
            case ChoiceUserInfo.Roles: this.GetRolesUserInfo(ref embed, ctxUser); break;
            case ChoiceUserInfo.Subscription: embed.WithDescription("Not implemented, sorry."); break;
        }

        await this.RespondAsync(embed: embed.Build());
    }

    private void GetNormalUserInfo(ref EmbedBuilder embed, IUser user)
    {
        SocketGuildUser? guildUser = this.Context.Guild.GetUser(user.Id);
        DiscordTimestamp createdAt = new DiscordTimestamp(user.CreatedAt);
        DiscordTimestamp joinedAt = new DiscordTimestamp(guildUser.JoinedAt!.Value);
        embed.AddField("ID", user.Id, true);

        if (guildUser != null)
        {
            embed
                .AddField("Nickname", guildUser.Nickname ?? "No nickname", true)
                .AddField($"Roles [{guildUser.Roles.Count()}]", "To see user roles use `/userinfo user:<user> menu:Roles`")
                .AddField("Join Date", $"{joinedAt.LongDateTime()} ({joinedAt.Relative()})");
        }

        embed
            .AddField("Account Created", $"{createdAt.LongDateTime()} ({createdAt.Relative})")
            .AddField("Subscription", "Coming soon!");
    }

    private void GetRolesUserInfo(ref EmbedBuilder embed, IUser user)
    {
        SocketGuildUser? guildUser = this.Context.Guild.GetUser(user.Id);
        embed.WithAuthor($"Roles for {user.ToString()}");

        if (guildUser != null)
        {
            var roles = guildUser.Roles.Select(x => $"<@&{x.Id}>");
            embed.WithDescription(string.Join(", ", roles));
        }
        else embed.WithDescription("This command can only executed in guild or user not found");
    }
}