using CoffeeBreak.Function;
using CoffeeBreak.Models;
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
        private DatabaseContext _db = new DatabaseContext();
        private DiscordShardedClient _client;
        public GiveawayCommand(DiscordShardedClient client)
        {
            _client = client;
        }

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [SlashCommand("start", "Start the giveaway.")]
        public async Task Start()
        {
            var checkChannel = await _db.GiveawayConfig.FirstOrDefaultAsync(x => x.GuildID == this.Context.Guild.Id);
            if (checkChannel == null)
            {
                await this.RespondAsync(
                    "Before using this command, set your giveaway channel using `/giveaway channel`.",
                    ephemeral: true);
                return;
            }

            // Make Select Menu
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Select an option").WithCustomId("select_menu_giveaway")
                .WithMinValues(1).WithMaxValues(1)
                .AddOption("Role Required", "role_required", "Choose if entries must have role")
                .AddOption("No role limit", "role_none");
            var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);
            await this.RespondAsync(
                "Before you make a giveaway, please choose below:",
                components: builder.Build(),
                ephemeral: true);
        }

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [SlashCommand("channel", "Set channel giveaway")]
        public async Task Channel(
            [ChannelTypes(ChannelType.Text)][Summary(description: "Channel target for giveaway")]
            ITextChannel? channel = null)
        {
            channel = channel == null ? this.Context.Channel as ITextChannel : channel;
            if (channel == null)
            {
                await this.RespondAsync("Channel not valid, please try again.");
                return;
            }

            // Check if giveaway is running
            var gaList = await _db.GiveawayRunning.Include(m => m.GiveawayConfig)
                .Where(x => x.GiveawayConfig.GuildID == this.Context.Guild.Id && x.IsExpired == false).ToArrayAsync();
            if (gaList.Count() > 0)
            {
                await this.RespondAsync(
                    $"Please wait or stop all the giveaway running in the <#{gaList.First().GiveawayConfig.ChannelID}> before executing this command.",
                    ephemeral: true);
                return;
            }

            var entity = await _db.GiveawayConfig.FirstOrDefaultAsync(x => x.GuildID == this.Context.Guild.Id);
            if (entity != null)
            {
                entity.ChannelID = channel.Id;
                _db.Update(entity);
            }
            else
            {
                await _db.GiveawayConfig.AddAsync(new Models.GiveawayConfig
                {
                    GuildID = this.Context.Guild.Id,
                    ChannelID = channel.Id
                });
            }
            await _db.SaveChangesAsync();
            await this.RespondAsync($"<#{this.Context.Channel.Id}> successfully set as Giveaway Channel.");
        }

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [SlashCommand("stop", "Stop the giveaway and roll the winner")]
        public async Task Stop(
            [Summary(description: "Giveaway ID - You can peek the ID from the bottom of giveaway")] ulong id)
            => await this.StopCancelAsync(id);

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [SlashCommand("cancel", "Cancel the giveaway")]
        public async Task Cancel(
            [Summary(description: "Giveaway ID - You can peek the ID from the bottom of giveaway")] ulong id)
            => await this.StopCancelAsync(id, true);

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [SlashCommand("edit", "Edit the giveaway")]
        public async Task Edit(
            [Summary(description: "Giveaway ID")] ulong id,
            [Summary(description: "Giveaway name")] string? giveawayName = null,
            [Summary(description: "Giveaway note")] string? giveawayNote = null,
            [Summary(description: "Winner count")] int winnerCount = 0)
        {
            var data = await _db.GiveawayRunning.Include(m => m.GiveawayConfig).FirstOrDefaultAsync(x => x.ID == id);
            bool isAdmin = this.Context.Guild.GetUser(this.Context.User.Id).Roles.Where(x => x.Permissions.Has(GuildPermission.Administrator)).Count() > 0;
            if (data == null)
            {
                await this.RespondAsync("ID not found. Please try again.", ephemeral: true);
                return;
            }
            if (this.Context.Guild.Id != data.GiveawayConfig.GuildID)
            {
                await this.RespondAsync("ID not found in this server. Please try again.", ephemeral: true);
                return;
            }
            if (!isAdmin && this.Context.User.Id != data.UserID)
            {
                await this.RespondAsync("You cannot modify that giveaway because you're not the creator or guild administrator", ephemeral: true);
                return;
            }

            if
            (
                // Set if to tabbed because we don't know next update the props will added
                giveawayName == null && giveawayNote == null
                && winnerCount == 0
            )
            {
                await this.RespondAsync("Please complete your command.", ephemeral: true);
                return;
            }

            // Modify to database
            if (giveawayName != null) data.GiveawayName = giveawayName;
            if (giveawayNote != null) data.GiveawayNote = giveawayNote;
            if (winnerCount != 0) data.WinnerCount = winnerCount;
            await _db.SaveChangesAsync();

            // Modify message
            var channel = this.Context.Guild.GetChannel(data.GiveawayConfig.ChannelID) as SocketTextChannel;
            if (channel == null)
            {
                await this.RespondAsync("Channel not found, please try again.", ephemeral: true);
                return;
            }
            await channel.ModifyMessageAsync(data.MessageID, Message => Message.Embed = GiveawayManager.GenerateEmbed(data, _client));

            await this.RespondAsync("Giveaway has been modified!", ephemeral: true);
        }
        
        private async Task StopCancelAsync(ulong id, bool isCanceled = false)
        {
            var data = await _db.GiveawayRunning.Include(m => m.GiveawayConfig).FirstOrDefaultAsync(x => x.ID == id);
            bool isAdmin = this.Context.Guild.GetUser(this.Context.User.Id).Roles.Where(x => x.Permissions.Has(GuildPermission.Administrator)).Count() > 0;
            if (data == null)
            {
                await this.RespondAsync("ID not found. Please try again.", ephemeral: true);
                return;
            }
            if (this.Context.Guild.Id != data.GiveawayConfig.GuildID)
            {
                await this.RespondAsync("ID not found in this server. Please try again.", ephemeral: true);
                return;
            }
            if (!isAdmin && this.Context.User.Id != data.UserID)
            {
                await this.RespondAsync("You cannot cancel that giveaway because you're not the creator or guild administrator", ephemeral: true);
                return;
            }

            SocketTextChannel? channel = this.Context.Guild.GetTextChannel(data.GiveawayConfig.ChannelID);
            if (channel == null) return;
            var message = await channel.GetMessageAsync(data.MessageID);
            if (message == null) return;

            Global.State.Giveaway.GiveawayActive.Remove(id);
            await GiveawayManager.StopGiveawayAsync(this.Context.Guild, channel, message, _db, data.ID, isCanceled);
            await this.RespondAsync("Giveaway successfully " + (isCanceled ? "canceled!" : "stopped!"), ephemeral: true);
        }
    }
}
