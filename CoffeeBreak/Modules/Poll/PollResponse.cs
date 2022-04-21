using CoffeeBreak.Function;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class PollModule
{
    [ModalInteraction("modal_poll:single")]
    public async Task ModalSingleResponseAsync(PollManager.PollSingleChoiceModal modal)
    {
        await this.RespondAsync("Mantap");
    }

    [ModalInteraction("modal_poll:multiple")]
    public async Task ModalMultipleResponseAsync(PollManager.PollMultipleChoiceModal modal)
    {
        await this.RespondAsync("Mantap");
    }
}
