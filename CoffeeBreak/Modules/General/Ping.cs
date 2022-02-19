using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("ping", "Ping! - Show the ping / response time of the bot in ms")]
    public async Task Ping()
    {
        await this.RespondAsync("🏓 | Pong!");
        IUserMessage Message = await this.GetOriginalResponseAsync();
        long TimeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long botPing = TimeNow - Message.Timestamp.ToUnixTimeMilliseconds();
        await this.ModifyOriginalResponseAsync(Message => Message.Content = $"🏓 | Pong! - **{botPing}ms**");
    }
}
