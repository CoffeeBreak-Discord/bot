using Discord;
using Discord.WebSocket;

namespace CoffeeBreak.ThirdParty;
public class GiveawayManager
{
    public static async Task StopGiveaway(SocketGuild guild, IMessage message)
    {
        // TODO: Make trigger to end the giveaway
    }
    public static Embed StopGiveawayEmbed(EmbedBuilder embed, SocketUser? winner = null, bool isCanceled = false)
    {
        embed.WithTitle("Giveaway Ended!").WithColor(Color.Red);
        if (winner == null) embed.AddField("Winner",
            "No one winner because " + (isCanceled ? "it's cancelled by creator." : "no one entering the giveaway"));
        else embed.AddField("Winner",
            $"The winner is {winner.Mention}, congratulations!");

        return embed.Build();
    }
}
