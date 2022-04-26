using CoffeeBreak.Function;
using CoffeeBreak.ThirdParty;
using CoffeeBreak.Models;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class PollModule
{
    [ModalInteraction("modal_poll:single")]
    public async Task ModalSingleResponseAsync(PollManager.PollSingleChoiceModal modal)
    {
        PollRunning data = new PollRunning()
        {
            ExpiredDate = new HumanizeDuration(modal.Duration).ToDateTime(),
            ChoiceCount = 1,
            GuildID = this.Context.Guild.Id,
            UserID = this.Context.User.Id,
            PollName = modal.PollName
        };
        
        // // Send message to channel
        // var message = await this.Context.Channel.SendMessageAsync("Creating poll..");
        // data.MessageID = message!.Id;

        // // Save the data before because we need ID
        // await _db.PollRunning.AddAsync(data);
        // await _db.SaveChangesAsync();
        // await this.RespondAsync("Poll successfully created!", ephemeral: true);
    }

    [ModalInteraction("modal_poll:multiple")]
    public async Task ModalMultipleResponseAsync(PollManager.PollMultipleChoiceModal modal)
    {
        PollRunning data = new PollRunning()
        {
            ExpiredDate = new HumanizeDuration(modal.Duration).ToDateTime(),
            ChoiceCount = int.Parse(modal.CountChoice),
            GuildID = this.Context.Guild.Id,
            UserID = this.Context.User.Id,
            PollName = modal.PollName
        };

        // Send message to channel
        var message = await this.Context.Channel.SendMessageAsync("Creating poll..");
        data.MessageID = message!.Id;

        // Save the data before because we need ID
        await _db.PollRunning.AddAsync(data);
        await _db.SaveChangesAsync();
        await this.RespondAsync("Poll successfully created!", ephemeral: true);
    }
}
