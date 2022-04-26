using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Function;
public class PollManager
{
    public class SingleChoiceModal : IModal
    {
        public string Title => "Make a single poll choice!";

        [InputLabel("Insert your poll name")]
        [ModalTextInput("poll_name", placeholder: "Ex: What's your favorite fruit?")]
        public string Name { get; set; } = default!;

        [InputLabel("How long this giveaway?")]
        [ModalTextInput("poll_duration", placeholder: "Ex: 3d 4h 5m 10s")]
        public string Duration { get; set; } = default!;

        [InputLabel("Some note?")]
        [ModalTextInput("poll_choice", TextInputStyle.Paragraph, "Separate the choice with new line", 1, 255)]
        public string Choice { get; set; } = default!;
    }

    public enum PollChoiceType
    {
        Single, Multiple
    }

    public static string[] SplitStringToMenu(string text) => text.Split("\n");

    public static SelectMenuBuilder GenerateMenu(int id, PollChoiceType type, string[] option, int count = 1)
    {
        var builder = new SelectMenuBuilder()
            .WithPlaceholder("Select an option.")
            .WithCustomId($"menu_poll:{id}")
            .WithMinValues(1);

        if (type == PollChoiceType.Multiple) builder.WithMaxValues(count);
        else builder.WithMaxValues(1);

        return builder;
    }
}
