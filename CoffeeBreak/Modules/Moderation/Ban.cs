using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class ModerationModule
{
    [RequireBotPermission(GuildPermission.BanMembers)]
    [RequireUserPermission(GuildPermission.BanMembers)]
    [SlashCommand("ban", "Ban user")]
    public async Task Ban(
        [ChannelTypes(ChannelType.Text)] IChannel channel,
        [Summary(description: "Person who want to banned")] IUser user,
        [Summary(description: "Reason why be banned")] string reason = "No reason")
    {
        SocketGuildUser? guildUser = this.Context.Guild.GetUser(user.Id);
        // Check if command is executed in guild
        if (this.Context.Guild == null)
        {
            await this.RespondAsync("You can only using this module in Guild/Server.");
            return;
        }

        // Check if user didn't found
        if (guildUser == null)
        {
            await this.RespondAsync("User not found!");
            return;
        }

        // Check if user is urself, owner, or have role higher than executor/bot.
        var checkPerm = this.CheckModerationPermission(guildUser,
            new FilterPermission[]
            { FilterPermission.Himself, FilterPermission.Owner, FilterPermission.HigherRole, FilterPermission.HigherRoleBot });
        if (checkPerm.IsError)
        {
            await this.RespondAsync(checkPerm.Message);
            return;
        }

        await guildUser.BanAsync(reason: reason);
        await this.RespondAsync($"{guildUser} successfully banned with reason:\n```{reason ?? "No reason"}```");
    }


    [RequireBotPermission(GuildPermission.BanMembers)]
    [RequireUserPermission(GuildPermission.BanMembers)]
    [SlashCommand("unban", "Ban user")]
    public async Task Unban(
        [ChannelTypes(ChannelType.Text)] IChannel channel,
        [Summary(description: "Snowflake ID of user")] string userID)
    {
        // Check if command is executed in guild
        if (this.Context.Guild == null)
        {
            await this.RespondAsync("You can only using this module in Guild/Server.");
            return;
        }

        await this.Context.Guild.RemoveBanAsync(ulong.Parse(userID));
        await this.RespondAsync($"{userID} successfully unbanned!");
    }
}
