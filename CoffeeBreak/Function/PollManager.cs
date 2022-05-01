using CoffeeBreak.Models;
using CoffeeBreak.ThirdParty;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Function;
public class PollManager
{
    public static string IconURL = "https://media.discordapp.net/attachments/946050537814655046/968628429467549706/unknown.png?width=288&height=288";

    public class SingleChoiceModal : IModal
    {
        public string Title => "Make a single poll choice!";

        [InputLabel("Insert your poll name")]
        [ModalTextInput("poll_name", placeholder: "Ex: What's your favorite fruit?")]
        public string Name { get; set; } = default!;

        [InputLabel("How long this poll?")]
        [ModalTextInput("poll_duration", placeholder: "Ex: 3d 4h 5m 10s")]
        public string Duration { get; set; } = default!;

        [InputLabel("Insert your option here")]
        [ModalTextInput("poll_choice", TextInputStyle.Paragraph, "Separate the option with new line", 1, 255)]
        public string Choice { get; set; } = default!;
    }

    public class MultipleChoiceModal : IModal
    {
        public string Title => "Make a multiple poll choice!";

        [InputLabel("Insert your poll name")]
        [ModalTextInput("poll_name", placeholder: "Ex: What's your favorite fruit?")]
        public string Name { get; set; } = default!;

        [InputLabel("How long this poll?")]
        [ModalTextInput("poll_duration", placeholder: "Ex: 3d 4h 5m 10s")]
        public string Duration { get; set; } = default!;

        [InputLabel("How many choice user can choice")]
        [ModalTextInput("poll_choice_count", placeholder: "Ex: 3")]
        public string Count { get; set; } = default!;

        [InputLabel("Are all options required to choose? (1/0)")]
        [ModalTextInput("poll_is_options_required", placeholder: "1 = True / 0 = False")]
        public string IsOptionsRequired { get; set; } = default!;

        [InputLabel("Insert your option here")]
        [ModalTextInput("poll_choice", TextInputStyle.Paragraph, "Separate the option with new line", 1, 255)]
        public string Choice { get; set; } = default!;
    }

    public enum PollChoiceType
    {
        Single, Multiple
    }

    public static MessageComponent GenerateMenu(ulong id, PollChoiceType type, List<PollChoice> option, int count = 1)
    {
        var builder = new SelectMenuBuilder()
            .WithPlaceholder("Select an option.")
            .WithCustomId($"menu_poll:{id}")
            .WithMinValues(1)
            .WithMaxValues(count);

        foreach (PollChoice choice in option)
        {
            builder.AddOption(choice.ChoiceValue, choice.ID.ToString());
        }

        return new ComponentBuilder().WithSelectMenu(builder).Build();
    }

    public static async Task<List<PollChoice>> InsertToPollChoice(DatabaseContext db, PollRunning poller, string[] option)
    {
        // Insert to database
        var dataChoice = new List<PollChoice>();
        foreach (string choice in option)
        {
            dataChoice.Add(new PollChoice()
            {
                PollRunning = poller,
                ChoiceValue = choice
            });
        }
        await db.PollChoice.AddRangeAsync(dataChoice.ToArray());

        return dataChoice;
    }

    public static Embed StartEmbedBuilder(PollRunning poll, DiscordShardedClient client)
    {
        var endTime = new DiscordTimestamp(poll.ExpiredDate);
        var embed = new EmbedBuilder()
            .WithColor(Color.Green)
            .WithTitle("Poll Started!")
            .WithThumbnailUrl(IconURL)
            .WithCurrentTimestamp()
            .WithFooter($"ID: {poll.ID}", client.CurrentUser.GetAvatarUrl())
            .AddField("Poll Name", poll.PollName)
            .AddField("Creator", $"<@!{poll.UserID}>", true)
            .AddField("Entries", "0 vote", true)
            .AddField("Choice Count", $"{poll.ChoiceCount} Option{(poll.ChoiceCount > 1 ? "s" : "")} ({(poll.IsOptionsRequired ? "Required" : "Optional")})", true)
            .AddField("End Time", $"{endTime.Relative()} ({endTime.LongDateTime()})");
        return embed.Build();
    }

    public static async Task UpdateStats(DatabaseContext db, ISocketMessageChannel channel, PollRunning data)
    {
        var message = await channel.GetMessageAsync(data.MessageID);
        var embedBuilder = message.Embeds.First().ToEmbedBuilder();

        // Get count
        int votes = await db.PollParticipant.Include(e => e.PollRunning).Where(e => e.PollRunning == data).CountAsync();

        // Find index for entries
        var index = embedBuilder.Fields.FindIndex(x => x.Name == "Entries");
        embedBuilder.Fields[index].Value = $"{votes} vote{(votes > 1 ? "s" : "")}";
        
        // Modify the message
        await channel.ModifyMessageAsync(data.MessageID, Message => Message.Embed = embedBuilder.Build());
    }

    public static async Task StopPollAsync(SocketGuild guild, SocketTextChannel channel, IMessage message, DatabaseContext db, ulong id, bool isCanceled = false)
    {
        // Recursing include
        var data = await db.PollRunning
            .Include(m => m.PollChoice).Include(m => m.PollParticipant).ThenInclude(m => m.PollChoice)
            .FirstOrDefaultAsync(x => x.ID == id);
        var embed = message.Embeds.First().ToEmbedBuilder().WithColor(Color.Red);

        // Get poll count
        int pollSize = data!.PollParticipant.Count;
        string printEmbed = "";
        foreach (var choice in data!.PollChoice)
        {
            int count = choice.PollParticipant == null ? 0 : choice.PollParticipant.Where(x => x.PollChoice.ID == choice.ID).Count();
            printEmbed += $"• {choice.ChoiceValue} - {count} ({(((double)count / pollSize) * 100).ToString("0.00")}%)\n";
        }
        
        if (pollSize > 0)
            embed.AddField("Result", printEmbed.TrimEnd());
        else
            embed.AddField("Result", isCanceled ? "Canceled" : "No Participant");

        // Modify message
        var msgModifed = await channel.ModifyMessageAsync(message.Id, Message =>
        {
            Message.Embed = embed.Build();
            Message.Components = null;
        });

        if (isCanceled)
            await channel.SendMessageAsync(
                $"The poll is cancelled because action from poll creator.\nPlease ask <@!{data!.UserID}> for more information.",
                messageReference: new MessageReference(message.Id, channel.Id, guild.Id));
        else
            await channel.SendMessageAsync(
                $"The poll is completely collected about {pollSize} vote, check the result by clicking message reference above this text.\nPlease ask <@!{data!.UserID}> for more information.",
                messageReference: new MessageReference(message.Id, channel.Id, guild.Id));
    
        Global.State.Poll.PollActive.Remove(data.ID);
        data.IsExpired = true;
        db.PollRunning.Update(data);
        await db.SaveChangesAsync();
    }
}
