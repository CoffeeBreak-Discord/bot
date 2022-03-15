using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Modules;
public partial class ModerationModule
{
    [SlashCommand("warn", "Set warn from specified user")]
    public async Task Warn(
        [Summary(description: "Target user")] IUser user,
        [Summary(description: "Reason")] string reason = "No reason")
    {
        SocketGuildUser? guildUser = this.Context.Guild.GetUser(user.Id);

        // Check if command is executed in guild
        if (this.Context.Guild == null)
        {
            await this.RespondAsync("You can only use this module in a Guild/Server.");
            return;
        }

        // Check if user didn't found
        if (guildUser == null)
        {
            await this.RespondAsync("User not found!");
            return;
        }

        // Deny if this user warn himself or bot
        var checkPerm = this.CheckModerationPermission(guildUser,
            new FilterPermission[] { FilterPermission.Himself, FilterPermission.Bot });
        if (checkPerm.IsError)
        {
            await this.RespondAsync(checkPerm.Message);
            return;
        }

        // Insert data to database
        await _db.WarnList.AddAsync(new Models.WarnList
        {
            GuildID = this.Context.Guild.Id,
            UserID = guildUser.Id,
            ExecutorID = this.Context.User.Id,
            Reason = reason
        });
        await _db.SaveChangesAsync();
        await this.RespondAsync($"{guildUser.Mention} was successfully warned with reason:\n```{reason ?? "No reason"}```");
    }

    [SlashCommand("warnlist", "Get warn list from specified user")]
    public async Task Warnlist(
        [Summary(description: "Target user")] IUser user)
    {
        SocketGuildUser? guildUser = this.Context.Guild.GetUser(user.Id);

        // Check if command is executed in guild
        if (this.Context.Guild == null)
        {
            await this.RespondAsync("You can only use this module in a Guild/Server.");
            return;
        }

        // Check if user didn't found
        if (guildUser == null)
        {
            await this.RespondAsync("User not found!");
            return;
        }

        // Get data from database
        var data = await _db.WarnList.Where(x => x.GuildID == this.Context.Guild.Id && x.UserID == guildUser.Id).ToArrayAsync();
        if (data.Count() == 0)
        {
            await this.RespondAsync("No record found!");
            return;
        }

        var map = data.Select((x, i) => $"{i}. {this.Context.Guild.GetUser(x.ExecutorID)}: {x.Reason} [{x.Timestamp}]");
        await this.RespondAsync($"Warn list for {guildUser}:\n{string.Join("\n", map)}");
    }
}
