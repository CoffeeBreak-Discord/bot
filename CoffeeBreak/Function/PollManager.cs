using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Function;
public class PollManager
{
    public class PollTemplateModal
    {
        [InputLabel("Insert your poll name")]
        [ModalTextInput("poll_name", placeholder: "Ex: What's your favorite fruit?")]
        public string PollName { get; set; } = default!;

        [InputLabel("How long this poll?")]
        [ModalTextInput("poll_duration", placeholder: "Ex: 3d 4h 5m 10s")]
        public string Duration { get; set; } = default!;

        [InputLabel("Any choice?")]
        [ModalTextInput( "poll_choice", TextInputStyle.Paragraph, "Separate the choice with new line.", 1, 255)]
        public string Choice { get; set; } = default!;
    }

    public class PollSingleChoiceModal : PollTemplateModal, IModal
    {
        public string Title => "Make a single poll choice!";
    }

    public class PollMultipleChoiceModal : PollTemplateModal, IModal
    {
        public string Title => "Make a multiple poll choice!";
    }
}
