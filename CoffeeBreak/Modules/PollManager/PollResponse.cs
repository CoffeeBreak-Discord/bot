using CoffeeBreak.Function;
using CoffeeBreak.Models;
using CoffeeBreak.ThirdParty;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

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

        // If the number of choices is'nt sufficient
        if (choice.Count() != data.ChoiceCount)
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
