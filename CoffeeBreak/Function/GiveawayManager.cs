using CoffeeBreak.Models;
using CoffeeBreak.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Function;
public class GiveawayManager
{
    public class GiveawayModal : IModal
    {
        public string Title => "Make a Giveaway!";

        [InputLabel("Insert your giveaway name")]
        [ModalTextInput("giveaway_name", placeholder: "Ex: Free nitro 1 month.")]
        public string GiveawayName { get; set; } = default!;

        [InputLabel("How long this giveaway?")]
        [ModalTextInput("giveaway_duration", placeholder: "Ex: 3d 4h 5m 10s")]
        public string Duration { get; set; } = default!;

        [InputLabel("How much winner?")]
        [ModalTextInput("giveaway_winner_count", placeholder: "Ex: 1", initValue: "1", maxLength: 3)]
        public string Winner { get; set; } = default!;

        [InputLabel("Some note?")]
        [ModalTextInput(
            "giveaway_note", TextInputStyle.Paragraph,
            "Spill some note for this giveaway.", 1, 255, "No note.")]
        public string Note { get; set; } = default!;
    }

    public static Embed GenerateEmbed(GiveawayRunning data, DiscordShardedClient client)
    {
        DateTimeOffset endTime = data.ExpiredDate;
        List<string> parseRole = new List<string>();

        // If required role is available
        if (data.RequiredRole != null)
            parseRole.Add($"Required: <@&{data.RequiredRole}>");

        var embed = new EmbedBuilder()
            .WithTitle("Giveaway Started!")
            .WithColor(Color.Green)
            .WithThumbnailUrl("https://media.discordapp.net/attachments/946050537814655046/948615258078085120/tada.png?width=400&height=394")
            .WithCurrentTimestamp()
            .WithFooter($"ID: {data.ID}", client.CurrentUser.GetAvatarUrl())
            .WithDescription(data.GiveawayName)
            .AddField("Note", data.GiveawayNote)
            .AddField("Creator", $"<@!{data.UserID}>", true)
            .AddField("Role", parseRole.Count() == 0 ? "No minimum/required role." : string.Join("\n", parseRole.ToArray()), true)
            .AddField("End Date", $"<t:{endTime.ToUnixTimeSeconds()}:F> which is <t:{endTime.ToUnixTimeSeconds()}:R> from now")
            .AddField("Entries/Winner", $"0 people / {data.WinnerCount} winner", true);
        return embed.Build();
    }

    public static async Task StopGiveawayAsync(SocketGuild guild, SocketTextChannel channel, IMessage message, DatabaseService db, bool isCanceled = false)
    {
        var data = await db.GiveawayRunning
            .Include(m => m.GiveawayConfig)
            .Where(x => x.GiveawayConfig.GuildID == guild.Id && x.GiveawayConfig.ChannelID == channel.Id && x.MessageID == message.Id)
            .FirstOrDefaultAsync();
        var participant = await db.GiveawayParticipant.Where(x => x.GiveawayRunning == data).ToArrayAsync();
        ulong winner = participant.Count() > 0 ? participant[new Random().Next(participant.Count())].UserID : 0;
        var embed = GiveawayManager.StopGiveawayEmbed((message.Embeds.First() as Embed).ToEmbedBuilder(), winner, isCanceled);

        var msgModifed = await channel.ModifyMessageAsync(message.Id, Message => 
        {
            Message.Embed = embed;
            Message.Components = null;
        });

        if (isCanceled)
            await channel.SendMessageAsync(
                $"The giveaway is cancelled because action from giveaway creator.\nPlease ask <@!{data!.UserID}> for more information.",
                messageReference: new MessageReference(message.Id, channel.Id, guild.Id));
        else if (winner == 0)
            await channel.SendMessageAsync(
                $"No winner for this time because no one participating in this giveaway.\nPlease ask <@!{data!.UserID}> for more information.",
                messageReference: new MessageReference(message.Id, channel.Id, guild.Id));
        else
            await channel.SendMessageAsync(
                $"The winner of **{embed.Description}** is <@!{winner}>, congratulations!\nPlease ask <@!{data!.UserID}> for more information.",
                messageReference: new MessageReference(message.Id, channel.Id, guild.Id));

        data.IsExpired = true;
        db.GiveawayRunning.Update(data);
        db.GiveawayParticipant.RemoveRange(participant);
        await db.SaveChangesAsync();
    }

    public static Embed StopGiveawayEmbed(EmbedBuilder embed, ulong winner, bool isCanceled = false)
    {
        embed.WithTitle("Giveaway Ended!").WithColor(Color.Red);
        if (winner == 0) embed.AddField("Winner", $"No winner{(isCanceled ? " (Canceled)" : "")}", true);
        else embed.AddField("Winner", $"<@!{winner}>", true);

        return embed.Build();
    }
}
