using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Modules;
public partial class ModerationModule
{
    [SlashCommand("warnlist", "Get warn list from specified user")]
    public async Task Warnlist(IUser user)
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

        // Get data from database
        var data = await _db.WarnList.Where(x => x.UserID == guildUser.Id).ToArrayAsync();
        Console.WriteLine($"Data count: {data.Count()}");
    }
}