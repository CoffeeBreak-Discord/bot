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
            embed: embed.WithColor(randColor.R, randColor.G, randColor.B).WithDescription("🏓 | Pong!").Build());

        IUserMessage Message = await this.GetOriginalResponseAsync();
        long TimeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long botPing = TimeNow - Message.Timestamp.ToUnixTimeMilliseconds();

        await this.ModifyOriginalResponseAsync(
            Message => Message.Embed = embed.WithColor(Color.Green).WithDescription($"🏓 | Pong! - **{botPing}ms**").Build());
    }
}
