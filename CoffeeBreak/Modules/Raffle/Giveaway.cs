using System.Security.Cryptography;
using System.Text;
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
            [Summary(description: "Your id channel")] ulong channelID = 0)
        {
            channelID = channelID == 0 ? this.Context.Channel.Id : channelID;
            SocketGuildChannel channel = this.Context.Guild.GetChannel(channelID);
            await _db.GiveawayConfig.AddAsync(new Models.GiveawayConfig
            {
                GuildID = this.Context.Guild.Id,
                ChannelID = channelID
            });
            await _db.SaveChangesAsync();
            await this.RespondAsync($"<#{this.Context.Channel.Id}> successfully set as Giveaway Channel.");
        }
    }

    [ModalInteraction("ModalGiveaway")]
    public async Task GiveawayModalResponse(GiveawayModal modal)
    {
        try
        {
            // Initialize model first because we need some manipulate data
            Models.GiveawayRunning data = new Models.GiveawayRunning();
            data.GiveawayName = modal.GiveawayName;
            data.WinnerCount = int.Parse(modal.Winner);
            data.ExpiredDate = new HumanizeDuration(modal.Duration).ToDateTime();

            // Get users
            SocketUser? ctxUser = this.Context.Guild.GetUser(this.Context.User.Id);
            SocketUser? ownerUserId = this.Context.Guild.GetUser(this.Context.User.Id);
            var searchOnwer = this.Context.Guild.Users.Where(x => modal.Maker == x.ToString());
            if (searchOnwer.Count() > 0)
                ownerUserId = this.Context.Guild.GetUser(searchOnwer.First().Id);
            if (ctxUser == null || ownerUserId == null)
            {
                await this.RespondAsync("User not found, please check your input in line 4.", ephemeral: true);
                return;
            }
            data.UserExecutorID = ctxUser.Id;
            data.UserMakerID = ownerUserId.Id;

            // Get roles
            List<ulong> roles = new List<ulong>();
            if (modal.Role != "No role")
            {
                string[] listRoleString = modal.Role.Split(" || ").Select(x => x.Trim()).ToArray();
                foreach (string roleName in listRoleString)
                {
                    var role = this.Context.Guild.Roles.Where(x => x.Name == roleName);
                    if (role.Count() > 0) roles.Add(role.First().Id);
                }
            }
            data.Role = roles.Count == 0 ? null : string.Join(',', roles.ToArray());

            // Get channel id and send giveaway to channel because we need messageID
            var channelConfig = await _db.GiveawayConfig.Where(x => x.GuildID == this.Context.Guild.Id).FirstAsync();
            ulong channelID = channelConfig.ChannelID;
            SocketTextChannel? channel = this.Context.Guild.GetTextChannel(channelID);
            if (channel == null)
            {
                await this.RespondAsync(
                    "Channel not found, please set again with executing `/giveaway channel` to channel target",
                    ephemeral: true);
                return;
            }
            var message = await channel.SendMessageAsync(embed: this.GenerateEmbed(data));
            data.MessageID = message.Id;
            data.GiveawayConfig = channelConfig;
            
            // Edit message so we can get hash ID
            ComponentBuilder comp = new ComponentBuilder()
                .WithButton("Join! 🪅", $"modal_giveaway_join:", ButtonStyle.Success)
                .WithButton("Leave ❌", $"modal_giveaway_cancel:", ButtonStyle.Danger);
            await message.ModifyAsync(m => m.Components = comp.Build());

            // Send to database
            await _db.GiveawayRunning.AddAsync(data);
            await _db.SaveChangesAsync();
            await this.RespondAsync(
                $"Giveaway successfully created! Check <#{channelID}> to see your giveaway. "
                + $"If you want to edit some giveaway, you can use `/giveaway modify id: [param]`.",
                ephemeral: true);
            }
        catch (System.Exception ex)
        {
            Logging.Error($"{(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}\n{ex.StackTrace}", "Giveaway");
        }
    }

    private Embed GenerateEmbed(Models.GiveawayRunning running)
    {
        var ctxUser = this.Context.Guild.GetUser(running.UserExecutorID)!;
        var makerUser = this.Context.Guild.GetUser(running.UserMakerID)!;
        var role = running.Role == null ? "" : string.Join(", ", running.Role.Split(',').Select(x => $"<@&{x}>"));
        DateTimeOffset EndTime = running.ExpiredDate;

        EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithThumbnailUrl("https://media.discordapp.net/attachments/946050537814655046/948615258078085120/tada.png?width=400&height=394")
            .WithTitle("Giveaway Started!")
            .WithFooter($"Created by {ctxUser.ToString()}")
            .WithCurrentTimestamp()
            .WithDescription(running.GiveawayName)
            .AddField("Creator", makerUser.Mention, true)
            .AddField("Minimum Role", role == "" ? "No role" : role, role == "")
            .AddField("End Time", $"<t:{EndTime.ToUnixTimeSeconds()}:F> which is <t:{EndTime.ToUnixTimeSeconds()}:R> from now")
            .AddField("Entries/Winner", $"0 people / {running.WinnerCount} winner");
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

        [InputLabel("Who is the maker?")]
        [ModalTextInput(
            "giveaway_maker",
            placeholder: "Ex: Coffee Break#1307",
            initValue: "Myself")]
        public string Maker { get; set; } = default!;

        [InputLabel("Any minimum roles?")]
        [ModalTextInput("giveaway_roles",
            placeholder: "Divide role name with \" || \" and set \"No role\" if no minimum roles",
            initValue: "No role")]
        public string Role { get; set; } = default!;
    }
}
