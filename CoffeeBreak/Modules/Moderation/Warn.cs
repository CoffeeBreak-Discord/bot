using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using CoffeeBreak.Function;

namespace CoffeeBreak.Modules;
public partial class ModerationModule
{
    [RequireUserPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("warn", "Set warn from specified user")]
    public async Task WarnCommandAsync(
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
        await guildUser.SendMessageAsync($"You has been warned by {this.Context.User} with reason:\n```{reason ?? "No reason"}```");
        await this.RespondAsync($"{guildUser.Mention} was successfully warned with reason:\n```{reason ?? "No reason"}```");
    }

    [SlashCommand("warnlist", "Get warn list from specified user")]
    public async Task WarnlistCommandAsync(
        [Summary(description: "Target user")] IUser user)
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

        // Get data from database
        var data = await _db.WarnList.Where(x => x.GuildID == this.Context.Guild.Id && x.UserID == guildUser.Id).ToArrayAsync();
        if (data.Count() == 0)
        {
            await this.RespondAsync("No record found!");
            return;
        }

        var map = data
            .Select((x) => $"{x.ID}. {x.Reason} [{new DiscordTimestamp(x.Timestamp).LongDateTime()}] by {this.Context.Guild.GetUser(x.ExecutorID).Mention}");
        EmbedBuilder embed = new EmbedBuilder()
            .WithAuthor(name: $"Warn list for {guildUser}", iconUrl: this.Context.Guild.IconUrl)
            .WithThumbnailUrl(guildUser.GetDisplayAvatarUrl())
            .WithCurrentTimestamp()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithDescription(string.Join("\n", map))
            .WithFooter($"Requested by {this.Context.User}");
        await this.RespondAsync(embed: embed.Build());
    }
}
