using CoffeeBreak.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Function;
public class GiveawayManager
{
    public static async Task StopGiveawayAsync(SocketGuild guild, SocketTextChannel channel, IMessage message, DatabaseService db)
    {
        var data = await db.GiveawayRunning
            .Include(m => m.GiveawayConfig)
            .Where(x => x.GiveawayConfig.GuildID == guild.Id && x.GiveawayConfig.ChannelID == channel.Id && x.MessageID == message.Id)
            .FirstOrDefaultAsync();
        var participant = await db.GiveawayParticipant.Where(x => x.GiveawayRunning == data).ToArrayAsync();
        ulong winner = participant.Count() > 0 ? participant[new Random().Next(participant.Count())].UserID : 0;
        var embed = GiveawayManager.StopGiveawayEmbed((message.Embeds.First() as Embed).ToEmbedBuilder(), winner);

        var msgModifed = await channel.ModifyMessageAsync(message.Id, Message => Message.Embed = embed);
        if (winner == 0)
            await channel.SendMessageAsync(
                $"No winner for this time because no one participating in this giveaway.\nPlease ask <@!{data!.UserID}> for more information.",
                messageReference: new MessageReference(message.Id, channel.Id, guild.Id));
        else
            await channel.SendMessageAsync(
                $"The winner of **{embed.Description} is <@!{winner}>, congratulations!\nPlease ask <@!{data!.UserID}> for more information.",
                messageReference: new MessageReference(message.Id, channel.Id, guild.Id));

        data.IsExpired = true;
        db.GiveawayRunning.Update(data);
        db.GiveawayParticipant.RemoveRange(participant);
        await db.SaveChangesAsync();
    }

    public static Embed StopGiveawayEmbed(EmbedBuilder embed, ulong winner, bool isCanceled = false)
    {
        embed.WithTitle("Giveaway Ended!").WithColor(Color.Red);
        if (winner == 0) embed.AddField("Winner",
            "No one winner because " + (isCanceled ? "it's cancelled by creator." : "no one entering the giveaway"));
        else embed.AddField("Winner",
            $"The winner is <@!{winner}>, congratulations!");

        return embed.Build();
    }
}
