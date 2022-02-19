using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class ModerationModule
{
    [RequireBotPermission(GuildPermission.KickMembers)]
    [RequireUserPermission(GuildPermission.KickMembers)]
    [SlashCommand("kick", "Kick user")]
    public async Task Kick(
        [Summary(description: "Person who want to kicked")] IUser user,
        [Summary(description: "Reason why be kicked")] string reason = "No reason")
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

        await guildUser.KickAsync(reason);
        await this.RespondAsync($"{guildUser} successfully kicked with reason:\n```{reason ?? "No reason"}```");
    }
}