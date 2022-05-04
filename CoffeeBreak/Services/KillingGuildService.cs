using CoffeeBreak.Models;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Services;
public class KillingGuildService
{
    private DiscordShardedClient _client;
    private DatabaseContext _db = new DatabaseContext();

    public KillingGuildService(DiscordShardedClient client)
    {
        _client = client;

        client.LeftGuild += this.LeftGuild;
    }

    private async Task LeftGuild(SocketGuild guild)
    {
        // Part 1: Raffle/Giveaway
        // Disable all giveaway from that guild
        var dataGiveaway = await _db.GiveawayRunning
            .Include(Module => Module.GiveawayConfig)
            .Where(x => x.GiveawayConfig.GuildID == guild.Id).ToArrayAsync();
        if (dataGiveaway.Count() > 0)
        {
            for (int i = 0; i < dataGiveaway.Count(); i++)
            {
                // Delete from cache
                Global.State.Giveaway.GiveawayActive.Remove(dataGiveaway[i].ID);

                // Disable giveaway
                dataGiveaway[i].IsExpired = true;
            }
            _db.GiveawayRunning.UpdateRange(dataGiveaway);
        }

        // Part 2: VoiceManager/Stage
        // Delete configuration
        var dataStage = await _db.StageConfig.Where(x => x.GuildID == guild.Id).ToArrayAsync();
        if (dataStage.Count() > 0)
        {
            // Remove all stage config
            _db.StageConfig.RemoveRange(dataStage);
        }

        // Part 3: Poll
        // Force end the poll
        var dataPoll = await _db.PollRunning.Where(x => x.GuildID == guild.Id && !x.IsExpired).ToArrayAsync();
        if (dataPoll.Count() > 0)
        {
            for (int i = 0; i < dataPoll.Count(); i++)
            {
                // Delete from cache
                Global.State.Poll.PollActive.Remove(dataPoll[i].ID);

                // Disable poll
                dataPoll[i].IsExpired = true;
            }
            _db.PollRunning.UpdateRange(dataPoll);
        }

        // Finalize the changes
        await _db.SaveChangesAsync();
    }
}
