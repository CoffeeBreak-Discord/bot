using CoffeeBreak.Function;
using CoffeeBreak.Models;
using CoffeeBreak.ThirdParty;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Services.Caching;
public class PollService
{
    private DatabaseContext _db = new DatabaseContext();
    private DiscordShardedClient _client;
    public PollService(DiscordShardedClient client)
    {
        _client = client;
        client.ShardReady += this.ShardReady;
    }

    public Task ShardReady(DiscordSocketClient client)
    {
        // Make this pool as async because if this pool is
        // non blockable, we afraid the data will executed more than 1 time.
        Interval.SetInterval(async () => await CachePool(), TimeSpan.FromSeconds(1));
        Logging.Info("Poll Pool is ready!", "PollPool");
        return Task.CompletedTask;
    }

    private async Task CachePool()
    {
        // Get minute from server, divide it from Giveaway Interval.
        // If result is 0, execute caching for database
#if DEBUG
        int now = int.Parse(DateTime.Now.ToString("ss"));
#else
        int now = int.Parse(DateTime.Now.ToString("mm"));
#endif
        int interval = Global.State.Giveaway.MinuteInterval;
        if (now % interval == 0) await this.FetchDatabaseAsync();

        // Last step, check giveaway cache
        await this.CheckGiveawayAsync();

        // And it's loop endless...
    }

    private async Task FetchDatabaseAsync()
    {
        // Get data that expired in this day
        DateTime endDate = DateTime.Now.AddDays(1);
    }

    private async Task CheckGiveawayAsync()
    {
        //
    }
}
