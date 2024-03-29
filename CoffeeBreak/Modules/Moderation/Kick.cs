using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class ModerationModule
{
    [RequireBotPermission(GuildPermission.KickMembers)]
    [RequireUserPermission(GuildPermission.KickMembers)]
    [SlashCommand("kick", "Kick user")]
    public async Task KickCommandAsync(
        [Summary(description: "Target user")] IUser user,
        [Summary(description: "Reason")] string reason = "No reason")
    {
        SocketGuildUser? guildUser = this.Context.Guild.GetUser(user.Id);

        // Check if command is executed in guild
        if (this.Context.Guild == null)
        {
            await this.RespondAsync(
                "You can't use this command because this command only can executed in text channel.",
                ephemeral: true);
            return;
        }

        // Check if user didn't found
        if (guildUser == null)
        {
            await this.RespondAsync("User not found!", ephemeral: true);
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

        await guildUser.SendMessageAsync($"You has been kicked by {this.Context.User} with reason:\n```{reason ?? "No reason"}```");
        await guildUser.KickAsync(reason);
        await this.RespondAsync($"Successfully kicked {guildUser} with reason:\n```{reason ?? "No reason"}```");
    }
}
