using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CoffeeBreak.ThirdParty.Discord;
public class RoleManager
{
    ///<summary>Get the highest role from the user.</summary>
    ///<returns>Will return a specific highest role or @everyone role
    ///if target user didn't have any role.</returns>
    ///<param name="user">Target user</param>
    public static SocketRole GetHighestRole(SocketGuildUser user)
    {
        return user.Roles.OrderByDescending(x => x.Position).ToArray()[0];
    }

    private static bool IsExecutable(SocketGuildUser userContext, SocketGuildUser userTarget)
        => GetHighestRole(userContext).Position > GetHighestRole(userTarget).Position;
    public static bool IsKickable(SocketGuildUser userContext, SocketGuildUser userTarget) => IsExecutable(userContext, userTarget);
    public static bool IsBannable(SocketGuildUser userContext, SocketGuildUser userTarget) => IsExecutable(userContext, userTarget);

    ///<summary>Check user if have the specified role based by name or id</summary>
    ///<returns>Will return role if found, otherwise return null.</returns>
    ///<param name="user">Target user</param>
    ///<param name="roleName">Role name</param>
    ///<param name="roleID">Role id</param>
    public static SocketRole? HasRole(SocketGuildUser user, string roleName = "", ulong roleID = 0)
    {
        var search = roleID != 0 ? user.Roles.Where(x => x.Id == roleID) : user.Roles.Where(x => x.Name == roleName);
        return search.Count() > 0 ? search.ToArray()[0] : null;
    }
}