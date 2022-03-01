using CoffeeBreak.ThirdParty.Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class RaffleModule
{
    [Group("giveaway", "Manage giveaway on your server.")]
    public class GiveawayCommand : InteractionModuleBase<ShardedInteractionContext>
    {
        [SlashCommand("start", "Start the giveaway.")]
        public async Task Start()
            => await this.Context.Interaction.RespondWithModalAsync<GiveawayModal>("ModalGiveaway");
    }

    [ModalInteraction("ModalGiveaway")]
    public async Task GiveawayModalResponse(GiveawayModal modal)
    {
        Console.WriteLine(new HumanizeDuration(modal.Duration).ToDateTime().ToString("dd/MM/yyyy HH:mm:ss"));
        await this.RespondAsync("Mantap", ephemeral: true);
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

        [InputLabel("Who is the maker?")]
        [ModalTextInput(
            "giveaway_maker",
            placeholder: "Ex: Coffee Break#1307",
            initValue: "Myself")]
        public string Maker { get; set; } = default!;

        [InputLabel("Any minimum roles?")]
        [ModalTextInput("giveaway_roles",
            placeholder: "Divide role name with \"||\" and set \"No role\" if no minimum roles",
            initValue: "No role")]
        public string Role { get; set; } = default!;
    }
}
