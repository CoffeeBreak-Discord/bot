using CoffeeBreak.Services;
using CoffeeBreak.ThirdParty;
using CoffeeBreak.ThirdParty.Discord;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Modules;
public partial class RaffleModule
{
    [Group("giveaway", "Manage giveaway on your server.")]
    public class GiveawayCommand : InteractionModuleBase<ShardedInteractionContext>
    {
        private DatabaseService _db;
        public GiveawayCommand(DatabaseService db)
        {
            _db = db;
        }

        [SlashCommand("start", "Start the giveaway.")]
        public async Task Start()
        {
            var checkChannel = await _db.GiveawayConfig.Where(x => x.GuildID == this.Context.Guild.Id).ToArrayAsync();
            if (checkChannel.Count() == 0)
            {
                await this.RespondAsync(
                    "Before using this command, set your giveaway channel using `/giveaway channel`.",
                    ephemeral: true);
                return;
            }

            await this.Context.Interaction.RespondWithModalAsync<GiveawayModal>("ModalGiveaway");
        }

        [SlashCommand("channel", "Set channel giveaway")]
        public async Task Channel(
            [ChannelTypes(ChannelType.Text)][Summary(description: "Channel target for giveaway")]
            ITextChannel? channel = null)
        {
            channel = channel == null ? this.Context.Channel as ITextChannel : channel;
            if (channel == null)
            {
                await this.RespondAsync("Channel not valid, please try again.", ephemeral: true);
                return;
            }

            await _db.GiveawayConfig.AddAsync(new Models.GiveawayConfig
            {
                GuildID = this.Context.Guild.Id,
                ChannelID = channel.Id
            });
            await _db.SaveChangesAsync();
            await this.RespondAsync($"<#{this.Context.Channel.Id}> successfully set as Giveaway Channel.");
        }
    }

    [ModalInteraction("ModalGiveaway")]
    public async Task GiveawayModalResponse(GiveawayModal modal)
    {
        await this.RespondAsync("Hello world");
    }

    private Embed GenerateEmbed(Models.GiveawayRunning running)
    {
        var embed = new EmbedBuilder();
        return embed.Build();
    }

    public class GiveawayModal : IModal
    {
        public string Title => "Make a Giveaway!";

        [InputLabel("Insert your giveaway name")]
        [ModalTextInput("giveaway_name", placeholder: "Ex: Free nitro 1 month.")]
        public string GiveawayName { get; set; } = default!;

        [InputLabel("How long this giveaway?")]
        [ModalTextInput("giveaway_duration", placeholder: "Ex: 3d 4h 5m 10s")]
        public string Duration { get; set; } = default!;

        [InputLabel("How much winner?")]
        [ModalTextInput("giveaway_winner_count", placeholder: "Ex: 1", initValue: "1")]
        public string Winner { get; set; } = default!;

        [InputLabel("Some note?")]
        [ModalTextInput(
            "giveaway_note", TextInputStyle.Paragraph,
            "Spill some note for this giveaway.", 1, 255)]
        public string Note { get; set; } = default!;
    }
}
