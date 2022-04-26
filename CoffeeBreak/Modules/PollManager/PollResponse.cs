using CoffeeBreak.Function;
using CoffeeBreak.ThirdParty;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Modules;
public partial class RaffleGiveawayModule
{
    [ModalInteraction("modal_poll:single", true)]
    public async Task ModalSingleResponseAsync(PollManager.SingleChoiceModal modal)
    {
        await this.RespondAsync(modal.Name + " Single");
    }

    [ModalInteraction("modal_poll:multiple", true)]
    public async Task ModalMultipleResponseAsync(PollManager.MultipleChoiceModal modal)
    {
        await this.RespondAsync(modal.Name + " Multiple");
    }
}
