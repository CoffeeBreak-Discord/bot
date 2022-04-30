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
        await this.CheckPollAsync();

        // And it's loop endless...
    }

    private async Task FetchDatabaseAsync()
    {
        // Get data that expired in this day
        DateTime endDate = DateTime.Now.AddDays(1);
        var dataFetched = await _db.PollRunning
            .Where(x => x.IsExpired == false && x.ExpiredDate < endDate)
            .ToArrayAsync();
        if (dataFetched.Count() == 0) return;
        foreach (var data in dataFetched)
        {
            // If the poll more than State.MinuteInterval, skip
            TimeSpan ts = data.ExpiredDate - DateTime.Now;
            int minDiff = (int) Math.Floor(ts.TotalMinutes);
            int interval = Global.State.Giveaway.MinuteInterval;
            if (minDiff >= interval) continue;

            // Insert to cache
            if (Global.State.Poll.PollActive.Where(x => x.Key == data.ID).Count() > 0) continue;

            Logging.Info($"Add ID:{data.ID} to State.Giveaway.GiveawayActive from database caching.", "GAPool");
            Global.State.Poll.PollActive.Add(data.ID, data.ExpiredDate);
        }
    }

    private async Task CheckPollAsync()
    {
        //
    }
}
