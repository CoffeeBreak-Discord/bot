using CoffeeBreak.Function;
using CoffeeBreak.Models;
using CoffeeBreak.ThirdParty;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Modules;
public partial class RaffleGiveawayModule
{
    private async Task ModalResponseAsync(PollManager.SingleChoiceModal? singleChoice = null, PollManager.MultipleChoiceModal? multipleChoice = null)
    {
        var data = new PollRunning()
        {
            GuildID = this.Context.Guild.Id,
            UserID = this.Context.User.Id,
            ChannelID = this.Context.Channel.Id,
            ChoiceCount = 1
        };
        string choiceStr = "";

        if (singleChoice != null)
        {
            data.ExpiredDate = new HumanizeDuration(singleChoice.Duration).ToDateTime();
            data.PollName = singleChoice.Name;
            choiceStr = singleChoice.Choice;
        }
        else if (multipleChoice != null)
        {
            data.ChoiceCount = int.Parse(multipleChoice.Count);
            data.ExpiredDate = new HumanizeDuration(multipleChoice.Duration).ToDateTime();
            data.PollName = multipleChoice.Name;
            data.IsOptionsRequired = int.Parse(multipleChoice.IsOptionsRequired) == 1;
            choiceStr = multipleChoice.Choice;
        }

        string[] choiceList = choiceStr.Trim().Split("\n");
        if (choiceList.Count() < 2 || (multipleChoice != null && choiceList.Count() < (data.ChoiceCount + 1)))
        {
            await this.RespondAsync(
                $"You can't make a poll if the choices are under {(multipleChoice != null ? data.ChoiceCount : 2)} items.",
                ephemeral: true);
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

    [ModalInteraction("modal_poll:single", true)]
    public async Task ModalSingleResponseAsync(PollManager.SingleChoiceModal modal) => await this.ModalResponseAsync(singleChoice: modal);

    [ModalInteraction("modal_poll:multiple", true)]
    public async Task ModalMultipleResponseAsync(PollManager.MultipleChoiceModal modal) => await this.ModalResponseAsync(multipleChoice: modal);

    [ComponentInteraction("menu_poll:*", true)]
    public async Task MenuChoiceResponseAsync(string ids, string[] choice)
    {
        ulong id = ulong.Parse(ids);
        var data = await _db.PollRunning.Include(e => e.PollChoice).Include(e => e.PollParticipant).FirstOrDefaultAsync(x => x.ID == id);
        if (data == null) return;

        // If user already poll
        if (data.PollParticipant.Where(x => x.UserID == this.Context.User.Id).Count() > 0)
        {
            await this.RespondAsync("You're already vote this poll, thank you.", ephemeral: true);
            return;
        }

        // If the number of choices isn't sufficient when options is required
        if (data.IsOptionsRequired && choice.Count() != data.ChoiceCount)
        {
            await this.RespondAsync("The number of your selections is not sufficient to record.", ephemeral: true);
            return;
        }

        // List poll no matter the poll is single or multiple
        var participants = new List<PollParticipant>();
        foreach (string option in choice)
        {
            participants.Add(new PollParticipant()
            {
                PollRunning = data,
                UserID = this.Context.User.Id,
                PollChoice = data.PollChoice.Where(x => x.ID == ulong.Parse(option)).First()
            });
        }
        await _db.PollParticipant.AddRangeAsync(participants.ToArray());
        await _db.SaveChangesAsync();
        await PollManager.UpdateStats(_db, this.Context.Channel, data);
        await this.RespondAsync("Your vote is successfully recorded.", ephemeral: true);
    }
}
