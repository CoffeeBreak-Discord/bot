using CoffeeBreak.Function;
using CoffeeBreak.Models;
using CoffeeBreak.ThirdParty;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Services.Caching;
public class GiveawayService
{
    private DatabaseContext _db = new DatabaseContext();
    private DiscordShardedClient _client;
    public GiveawayService(DiscordShardedClient client)
    {
        _client = client;
        client.ShardReady += this.ShardReady;
    }

    public Task ShardReady(DiscordSocketClient client)
    {
        // Make this pool as async because if this pool is
        // non blockable, we afraid the data will executed more than 1 time.
        Interval.SetInterval(async () => await CachePool(), TimeSpan.FromSeconds(1));
        Logging.Info("Giveaway Pool is ready!", "GAPool");
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
        var dataFetched = await _db.GiveawayRunning
            .Include(m => m.GiveawayConfig)
            .Where(x => x.IsExpired == false && x.ExpiredDate < endDate)
            .ToArrayAsync();
        if (dataFetched.Count() == 0) return;
        foreach (var data in dataFetched)
        {
            // If the giveaway more than State.Giveaway.MinuteInterval, skip
            TimeSpan ts = data.ExpiredDate - DateTime.Now;
            int minDiff = (int) Math.Floor(ts.TotalMinutes);
            int interval = Global.State.Giveaway.MinuteInterval;
            if (minDiff >= interval) continue;

            // Insert to cache
            if (Global.State.Giveaway.GiveawayActive.Where(x => x.Key == data.ID).Count() > 0) continue;

            Logging.Info($"Add ID:{data.ID} to State.Giveaway.GiveawayActive from database caching.", "GAPool");
            Global.State.Giveaway.GiveawayActive.Add(data.ID, data.ExpiredDate);
        }
    }

    private async Task CheckGiveawayAsync()
    {
        // Set to list because Collection impossible to loop the value
        // if the Collection make changed
        foreach (var giveaway in Global.State.Giveaway.GiveawayActive.ToList())
        {
            // Get data, for scoping purposes
            var data = await _db.GiveawayRunning.Include(m => m.GiveawayConfig).Include(m => m.GiveawayParticipant)
                .Where(x => x.ID == giveaway.Key)
                .FirstOrDefaultAsync();

            // Get guild, channel, and message
            var guild = _client.GetGuild(data!.GiveawayConfig.GuildID);
            // TODO: If guild is null, delete all data based on channel
            if (guild == null) continue;
            var channel = guild.GetChannel(data!.GiveawayConfig.ChannelID) as SocketTextChannel;
            if (channel == null) continue;

            // Check diff before get message because we afraid discord
            // connection will slow
            var msVal = ((DateTimeOffset)giveaway.Value).ToUnixTimeSeconds();
            if (msVal - DateTimeOffset.Now.ToUnixTimeSeconds() > 0) continue;

            // Get message, if null remove to cache because is unecessary
            var message = await channel.GetMessageAsync(data.MessageID);
            if (message == null)
            {
                Logging.Warning($"Invalid giveaway from ID:{giveaway.Key}. Removing from database.", "GAPool");
                Global.State.Giveaway.GiveawayActive.Remove(giveaway.Key);

                if (data == null) continue;
                if (data.GiveawayParticipant != null && data.GiveawayParticipant.Count() > 0)
                    _db.GiveawayParticipant.RemoveRange(data.GiveawayParticipant);
                _db.GiveawayRunning.Remove(data);
                await _db.SaveChangesAsync();
                continue;
            }

            // If safe from anything, execute stop giveaway
            Logging.Info($"Stop giveaway for ID:{giveaway.Key}.", "GAPool");
            await GiveawayManager.StopGiveawayAsync(guild, channel, message, _db, giveaway.Key);
        }
    }
}
