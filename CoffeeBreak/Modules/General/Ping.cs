using CoffeeBreak.ThirdParty.StringModifier;
using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("ping", "Ping! - Show the ping / response time of the bot in ms")]
    public async Task PingCommandAsync()
    {
        EmbedBuilder embed = new EmbedBuilder();
        var randColor = Global.BotColors.Randomize();

        await this.RespondAsync(
            embed: embed.WithColor(randColor.IntCode).WithDescription("🏓 | Wait for it...").Build());

        // Get offset ms from pinging message
        IUserMessage Message = await this.GetOriginalResponseAsync();
        long TimeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long botPing = TimeNow - Message.Timestamp.ToUnixTimeMilliseconds();

        // Fix if ms < 0 so the result cannot be minus
        botPing = botPing < 0 ? botPing * -1 : botPing;

        // Get offset ms from database pinging
        TimeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long dbPing = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await _db.Database.CanConnectAsync();
        dbPing = (TimeNow - dbPing) + 1;

        // Get offset ms from redis pinging
        TimeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long cachePing = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await _cache.GetDatabase().PingAsync();
        cachePing = (TimeNow - cachePing) + 1;

        // Print it
        string desc = DictTabbed.Generate(new Dictionary<string, string>
        {
            { "Discord", $"{botPing}ms" },
            { "Database", $"{dbPing}ms" },
            { "Cache Pool", $"{cachePing}ms" }
        }, DictTabbed.Align.Right);
        await this.ModifyOriginalResponseAsync(
            Message => Message.Embed = embed.WithColor(Color.Green).WithDescription($"🏓 | **Pong!**\n```{desc}```").Build());
    }
}
