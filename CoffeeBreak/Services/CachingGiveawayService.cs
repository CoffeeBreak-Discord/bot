using CoffeeBreak.Function;
using CoffeeBreak.ThirdParty;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Services;
public class CachingGiveawayService
{
    private DatabaseService _db;
    private DiscordShardedClient _client;
    public CachingGiveawayService(DiscordShardedClient client, DatabaseService db)
    {
        _db = db;
        _client = client;
        client.ShardReady += this.ShardReady;
    }

    public async Task ShardReady(DiscordSocketClient client)
    {
        // Get cache first before looping
        await FetchDatabaseAsync();
        // Make this pool as async because if this pool is
        // non blockable, we afraid the data will executed more than 1 time.
        Interval.SetInterval(async () => await CachePool(), 1000);
        Logging.Info("Giveaway Pool is ready!", "GAPool");
    }

    private async Task CachePool()
    {
        // Get minute from server, divide it from Giveaway Interval.
        // If result is 0, execute caching for database
        int now = int.Parse(DateTime.Now.ToString("ss"));
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
            string key = $"{data.GiveawayConfig.GuildID}:{data.GiveawayConfig.ChannelID}:{data.MessageID}";
            if (Global.State.Giveaway.GiveawayActive.Where(x => x.Key == key).Count() > 0) continue;

            Logging.Info($"Add {key} to State.Giveaway.GiveawayActive from database caching.", "GAPool");
            Global.State.Giveaway.GiveawayActive.Add(
                $"{data.GiveawayConfig.GuildID}:{data.GiveawayConfig.ChannelID}:{data.MessageID}",
                data.ExpiredDate);
        }
    }

    private async Task CheckGiveawayAsync()
    {
        // Set to list because Collection impossible to loop the value
        // if the Collection make changed
        foreach (var giveaway in Global.State.Giveaway.GiveawayActive.ToList())
        {
            // Fetch id from key
            var keySplit = giveaway.Key.Split(':');
            ulong guildID = ulong.Parse(keySplit[0]);
            ulong channelID = ulong.Parse(keySplit[1]);
            ulong messageID = ulong.Parse(keySplit[2]);

            // Get data, for scoping purposes
            var data = await _db.GiveawayRunning.Include(m => m.GiveawayConfig).Include(m => m.GiveawayParticipant)
                .Where(x => x.GiveawayConfig.GuildID == guildID && x.GiveawayConfig.ChannelID == channelID && x.MessageID == messageID)
                .FirstOrDefaultAsync();

            // Get guild, channel, and message
            var guild = _client.GetGuild(guildID);
            // TODO: If guild is null, delete all data based on channel
            if (guild == null) continue;
            var channel = guild.GetChannel(channelID) as SocketTextChannel;
            if (channel == null) continue;

            // Check diff before get message because we afraid discord
            // connection will slow
            var msVal = ((DateTimeOffset)giveaway.Value).ToUnixTimeSeconds();
            if (msVal - DateTimeOffset.Now.ToUnixTimeSeconds() > 0) continue;

            // Get message, if null remove to cache because is unecessary
            var message = await channel.GetMessageAsync(messageID);
            if (message == null)
            {
                Logging.Warning($"Invalid giveaway from {guildID}:{channelID}[{messageID}]. Removing from database.", "GAPool");
                Global.State.Giveaway.GiveawayActive.Remove(giveaway.Key);
                
                if (data == null) continue;
                if (data.GiveawayParticipant != null && data.GiveawayParticipant.Count() > 0)
                    _db.GiveawayParticipant.RemoveRange(data.GiveawayParticipant);
                _db.GiveawayRunning.Remove(data);
                await _db.SaveChangesAsync();
                continue;
            }

            // If safe from anything, execute stop giveaway
            Logging.Info($"Stop giveaway for {guildID}:{channelID}[{messageID}].", "GAPool");
            Global.State.Giveaway.GiveawayActive.Remove(giveaway.Key);
            await GiveawayManager.StopGiveawayAsync(guild, channel, message, _db);
        }
    }
}
