using CoffeeBreak.Services;
using CoffeeBreak.ThirdParty.Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.Modules;
public partial class ModerationModule : InteractionModuleBase<ShardedInteractionContext>
{
    private DiscordShardedClient _client;
    private DatabaseService _db;

    public ModerationModule(DiscordShardedClient client, DatabaseService db)
    {
        _client = client;
        _db = db;
    }

    private class DeclinedPermissionContext
    {
        public bool IsError = false;
        public string Message = "No message";
    }

    private enum FilterPermission
    {
        Himself, Owner, HigherRole, HigherRoleBot
    }

    private DeclinedPermissionContext CheckModerationPermission(SocketGuildUser guildUser, FilterPermission[] filters)
    {
        var ctx = new DeclinedPermissionContext();
        SocketGuildUser ctxUser = this.Context.Guild.GetUser(this.Context.User.Id);
        SocketGuildUser clientUser = this.Context.Guild.GetUser(_client.CurrentUser.Id);

        // Function to get filter
        bool getFilter(FilterPermission filter) => filters.Where(x => x == filter).Count() > 0;

        // Set error first to saving some line
        ctx.IsError = true;
        ctx.Message = "This command cannot executed properly because the target is ";
        
        if (getFilter(FilterPermission.Himself) && this.Context.User.Id == guildUser.Id)
        {
            ctx.Message += "yourself.";
            return ctx;
        }
        if (getFilter(FilterPermission.Owner) && this.Context.Guild.OwnerId == guildUser.Id)
        {
            ctx.Message += "owner server.";
            return ctx;
        }
        if (this.Context.Guild.OwnerId != ctxUser.Id)
        {
            if (getFilter(FilterPermission.HigherRole) && !RoleManager.IsKickable(ctxUser, guildUser))
            {
                ctx.Message += "higher or same than you.";
                return ctx;
            }
            if (getFilter(FilterPermission.HigherRole) && !RoleManager.IsKickable(clientUser, guildUser))
            {
                ctx.Message += "higher or same from this bot.";
                return ctx;
            }
        }

        ctx.IsError = false;
        ctx.Message = "No error";
        return ctx;
    }
}