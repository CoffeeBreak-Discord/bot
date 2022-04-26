using CoffeeBreak.Function;
using CoffeeBreak.Models;
using CoffeeBreak.ThirdParty;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class RaffleGiveawayModule
{
    [ModalInteraction("modal_poll:single", true)]
    public async Task ModalSingleResponseAsync(PollManager.SingleChoiceModal modal)
    {
        var data = new PollRunning()
        {
            ChoiceCount = 1,
            ExpiredDate = new HumanizeDuration(modal.Duration).ToDateTime(),
            GuildID = this.Context.Guild.Id,
            PollName = modal.Name,
            UserID = this.Context.User.Id
        };

        string[] choiceList = modal.Choice.Trim().Split("\n");
        if (choiceList.Count() < 2)
        {
            await this.RespondAsync("You can't make a poll if the choices are under two items.", ephemeral: true);
            return;
        }

        // Save message ID and insert to database
        var message = await this.Context.Channel.SendMessageAsync("Creating poll...");
        data.MessageID = message.Id;
        await _db.PollRunning.AddAsync(data);

        // Insert data to PollChoice
        var dataChoice = await PollManager.InsertToPollChoice(_db, data, choiceList);
        await _db.SaveChangesAsync();

        // Generate poll
        await this.Context.Channel.ModifyMessageAsync(message.Id, Message =>
        {
            Message.Content = null;
            Message.Components = PollManager.GenerateMenu(data.ID, PollManager.PollChoiceType.Single, dataChoice, data.ChoiceCount);
            Message.Embed = PollManager.StartEmbedBuilder(data, _client);
        });

        await this.RespondAsync(
            $"Poll successfully created. If you want to cancel or stop the poll, you can use `/poll stop id:{data.ID}`.",
            ephemeral: true);
    }

    [ModalInteraction("modal_poll:multiple", true)]
    public async Task ModalMultipleResponseAsync(PollManager.MultipleChoiceModal modal)
    {
        await this.RespondAsync(modal.Name + " Multiple");
    }
}
