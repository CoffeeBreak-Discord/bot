using CoffeeBreak.Function;
using CoffeeBreak.ThirdParty;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Modules;
public partial class RaffleGiveawayModule
{
    [ModalInteraction("modal_poll:*", true)]
    public async Task ModalSingleResponseAsync(string mode, PollManager.SingleChoiceModal modal)
    {
        await this.RespondAsync("AAAAA");
    }
}
