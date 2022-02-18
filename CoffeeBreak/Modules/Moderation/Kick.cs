using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class ModerationModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("kick", "Kick user")]
    public async Task Kick(
        [Summary(description: "Person who want to kicked")] IUser user,
        [Summary(description: "Reason why be kicked")] string reason)
    {
        SocketGuildUser? guildUser = this.Context.Guild.GetUser(user.Id);
        SocketGuildUser ctxUser = this.Context.Guild.GetUser(this.Context.User.Id);
        SocketGuildUser clientUser = this.Context.Guild.GetUser(_client.CurrentUser.Id);

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
        if (this.Context.User.Id == user.Id)
        {
            await this.RespondAsync("You cannot kick yourself.");
            return;
        }
        if (this.Context.Guild.OwnerId == guildUser.Id)
        {
            await this.RespondAsync("You cannot kick an owner.");
            return;
        }
        if (this.Context.Guild.OwnerId != ctxUser.Id)
        {
            if (!this.IsExecutable(ctxUser, guildUser))
            {
                await this.RespondAsync("You cannot kick user that higher or same than you.");
                return;
            }
            if (!this.IsExecutable(clientUser, guildUser))
            {
                await this.RespondAsync("You cannot kick user that higher from this bot.");
                return;
            }
        }

        await guildUser.KickAsync(reason);
        await this.RespondAsync($"{guildUser} successfully kicked with reason:\n```{reason}```");
    }
}