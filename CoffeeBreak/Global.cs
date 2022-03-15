using CoffeeBreak.Modules;
using CoffeeBreak.ThirdParty;

namespace CoffeeBreak;
public class Global
{
    public class Bot
    {
        public static string ProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName ?? "CoffeeBreak";
        public static string PrettyName = "Coffee Break";
    }

    public class Constant
    {
        public readonly static ulong GUIDDevelopment = 937692307535302697;
        public readonly static string DiscordToken =
            Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? string.Empty;
        public readonly static int TotalShards =
            int.Parse(Environment.GetEnvironmentVariable("DISCORD_SHARD_COUNT") ?? "1");
    }

    public class Presence
    {
        public static readonly string[] Status =
        {
            "Party!",
            "Only interaction that we recieve _/\\_",
            "Keep calm."
        };
        public static readonly int Interval = 10;
    }

    public static ColorPallete BotColors = new ColorPallete(new ColorPallete.RGBPlate[]
    {
        new ColorPallete.RGBPlate(135, 100, 69),
        new ColorPallete.RGBPlate(202, 150, 92),
        new ColorPallete.RGBPlate(238, 195, 115),
        new ColorPallete.RGBPlate(244, 223, 186)
    });

    public class State
    {
        public class Giveaway
        {
            /// <summary>Interval for cache giveaway data in minute</summary>
            public static readonly int MinuteInterval = 5;

            /// <summary>
            /// Caching for Giveaway. Will updated every <see cref="MinuteInterval"/> that
            /// updated in each minutes.
            /// <example><code>
            /// GiveawayActive("guildID:channelID:messageID", expiredDate);
            /// </code></example>
            /// </summary>
            public static Dictionary<string, DateTime> GiveawayActive = new Dictionary<string, DateTime>();
        }
    }
}
