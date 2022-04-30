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

        // If the giveaway less that State.MinuteInterval, proceed to cache
        TimeSpan ts = poller.ExpiredDate - DateTime.Now;
        int minDiff = (int) Math.Floor(ts.TotalMinutes);
        int interval = Global.State.Poll.MinuteInterval;
        if (interval >= minDiff)
        {

            Logging.Info($"Add ID:{poller.ID} to State.Giveaway.GiveawayActive because the Giveaway less than {interval} minutes.", "GAPool");
            Global.State.Poll.PollActive.Add(poller.ID, poller.ExpiredDate);
        }

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
            .WithDescription(poll.PollName)
            .AddField("Creator", $"<@!{poll.UserID}>", true)
            .AddField("Entries", "0 people", true)
            .AddField("Choice Count", $"{poll.ChoiceCount} Option{(poll.ChoiceCount > 1 ? "s" : "")} ({(poll.IsOptionsRequired ? "Required" : "Optional")})", true)
            .AddField("End Time", $"{endTime.Relative()} ({endTime.LongDateTime()})");
        return embed.Build();
    }

    public static async Task UpdateStats(DatabaseContext db, ISocketMessageChannel channel, PollRunning data)
    {
        var message = await channel.GetMessageAsync(data.MessageID);
        var embedBuilder = message.Embeds.First().ToEmbedBuilder();

        // Get count
        int entries = await db.PollParticipant.Include(e => e.PollRunning).Where(e => e.PollRunning == data).CountAsync();

        // Find index for entries
        var index = embedBuilder.Fields.FindIndex(x => x.Name == "Entries");
        embedBuilder.Fields[index].Value = $"{entries} people{(entries > 1 ? "s" : "")}";
        
        // Modify the message
        await channel.ModifyMessageAsync(data.MessageID, Message => Message.Embed = embedBuilder.Build());
    }
}
