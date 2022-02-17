using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class ModerationModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("kick", "Kick user")]
    public async Task Kick([Summary(description: "The person who want to kicked")] IUser user)
    {
        // Check if command is executed in guild
        if (this.Context.Guild == null)
        {
            await this.RespondAsync("You can only using this module in Guild/Server");
            return;
        }

        var guildUser = this.Context.Guild.GetUser(user.Id);
        
        // Check if user is urself, owner, admin, or have role higher than executor/bot.
        if (this.Context.User.Id == user.Id)
        {
            await this.RespondAsync("You cannot kick yourself");
            return;
        }
        if (this.Context.Guild.OwnerId == guildUser.Id)
        {
            await this.RespondAsync("You cannot kick an owner");
            return;
        }
        if (guildUser.Roles.Where(x => x.Permissions.Has(GuildPermission.Administrator)).Count() > 0)
        {
            await this.RespondAsync("You cannot kick an admin");
            return;
        }

        var botRole = this.Context.Guild.GetUser(_client.CurrentUser.Id) .Roles.OrderByDescending(x => x.Position);
        var botPosition = botRole.Count() > 0 ? botRole.ToArray()[0].Position : -1;

        await this.RespondAsync($"You'll kick {guildUser}");
    }
}
