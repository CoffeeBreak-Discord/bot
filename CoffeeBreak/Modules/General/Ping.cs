using Discord.Interactions;
using Discord;

namespace CoffeeBreak.Modules;
public partial class GeneralModule : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("ping", "Ping!")]
    public async Task Ping()
    {
        await this.RespondAsync("🏓 | Pong!");
        IUserMessage Message = await this.GetOriginalResponseAsync();
        long TimeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long botPing = TimeNow - Message.Timestamp.ToUnixTimeMilliseconds();
        await this.ModifyOriginalResponseAsync(Message => Message.Content = $"🏓 | Pong! - **{botPing}ms**");
    }
}
