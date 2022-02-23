using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("ping", "Ping! - Show the ping / response time of the bot in ms")]
    public async Task Ping()
    {
        EmbedBuilder embed = new EmbedBuilder();
        var randColor = Global.BotColors.Randomize();

        await this.RespondAsync(
            embed: embed.WithColor(randColor.IntCode).WithDescription("🏓 | Pong!").Build());

        IUserMessage Message = await this.GetOriginalResponseAsync();
        long TimeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long botPing = TimeNow - Message.Timestamp.ToUnixTimeMilliseconds();
        
        // Fix if ms < 0 so the result cannot be minus
        botPing = botPing < 0 ? botPing * -1 : botPing;

        await this.ModifyOriginalResponseAsync(
            Message => Message.Embed = embed.WithColor(Color.Green).WithDescription($"🏓 | Pong! - **{botPing}ms**").Build());
    }
}
