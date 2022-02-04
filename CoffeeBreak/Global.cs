namespace CoffeeBreak
{
    public class Global
    {
        public class Constant
        {
            public readonly static ulong GUIDDevelopment = 937692307535302697;
            public readonly static string DiscordToken =
                Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? string.Empty;
            public readonly static int TotalShards =
                int.Parse(Environment.GetEnvironmentVariable("DISCORD_SHARD_COUNT") ?? "1");
        }
    }
}
