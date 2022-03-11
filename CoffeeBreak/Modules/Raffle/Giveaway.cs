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
    }

    // If user spawning giveaway, first of all user will teleport to
    // this interaction to parse the menu value.
    [ComponentInteraction("select_menu_giveaway")]
    public async Task GiveawaySelectRoleResponse(string[] selectedMenu)
    {
        string menu = selectedMenu[0];
        if (menu == "role_none")
        {
            await this.Context.Interaction.RespondWithModalAsync<GiveawayModal>("modal_giveaway:none;0");
            return;
        }

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select role").WithCustomId("select_menu_giveaway_role")
            .WithMinValues(1).WithMaxValues(1);
        foreach (var role in this.Context.Guild.Roles)
            menuBuilder.AddOption(role.Name, role.Id.ToString());
        var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);

        await this.RespondAsync(
            "Choose the role required below:\n*Dismiss this message if you want to cancel this command.*",
            components: builder.Build(),
            ephemeral: true);
    }

    // If user choose the role that required for giveaway, will teleport
    // to this interaction
    [ComponentInteraction("select_menu_giveaway_role")]
    public async Task GiveawaySelectVerifiedRequiredRoleResponse(string[] selectedMenu)
        => await this.Context.Interaction.RespondWithModalAsync<GiveawayModal>($"modal_giveaway:required;{selectedMenu[0] ?? "0"}");

    [ModalInteraction("modal_giveaway:*;*")]
    public async Task GiveawayModalResponse(string mode, string roleID, GiveawayModal modal)
    {
        Models.GiveawayRunning data = new Models.GiveawayRunning();
        data.GiveawayName = modal.GiveawayName;
        data.GiveawayNote = modal.Note;
        data.UserID = this.Context.User.Id;
        data.WinnerCount = int.Parse(modal.Winner);
        data.ExpiredDate = new HumanizeDuration(modal.Duration).ToDateTime();

        // Parse mode except for none
        if (mode == "required")
        {
            var role = this.Context.Guild.GetRole(ulong.Parse(roleID));
            if (role != null) data.RequiredRole = role.Id;
        }

        // Get channel id and send giveaway to channel
        var channelConf = await _db.GiveawayConfig.FirstOrDefaultAsync(x => x.GuildID == this.Context.Guild.Id);
        SocketTextChannel? channel = this.Context.Guild.GetTextChannel(channelConf!.ChannelID);
        if (channel == null)
        {
            await this.RespondAsync(
                "Channel not found, please set giveaway channel with executing `/giveaway channel`.",
                ephemeral: true);
            return;
        }
        var message = await channel.SendMessageAsync("Generate giveaway...");
        data.MessageID = message.Id;

        // Set button to giveaway message
        var comp = new ComponentBuilder()
            .WithButton("Turn me in! 🎉", $"button_giveaway_toggle:", ButtonStyle.Primary)
            .Build();
        await message.ModifyAsync(Message =>
        {
            Message.Components = comp;
            Message.Embed = this.GenerateEmbed(data);
            Message.Content = null;
        });
        data.GiveawayConfig = channelConf;

        // Save to database
        await _db.GiveawayRunning.AddAsync(data);
        await _db.SaveChangesAsync();

        // If the giveaway less than State.Giveaway.MinuteInterval, proceed to cache
        TimeSpan ts = data.ExpiredDate - DateTime.Now;
        int minDiff = (int) Math.Floor(ts.TotalMinutes);
        string key = $"{data.GiveawayConfig.GuildID}:{data.GiveawayConfig.ChannelID}:{data.MessageID}";
        int interval = Global.State.Giveaway.MinuteInterval;
        if (interval >= minDiff)
        {
            Logging.Info($"Add {key} to State.Giveaway.GiveawayActive because the Giveaway less than {interval} minutes.", "GAPool");
            Global.State.Giveaway.GiveawayActive.Add(
                $"{data.GiveawayConfig.GuildID}:{data.GiveawayConfig.ChannelID}:{data.MessageID}",
                data.ExpiredDate);
        }

        await this.RespondAsync(
            $"Giveaway successfully created! Check <#{data.GiveawayConfig.ChannelID}> to see your giveaway.\n"
            + $"If you want to edit some giveaway, you can use `/giveaway modify id:{data.MessageID} [param]`.",
            ephemeral: true);
    }

    private Embed GenerateEmbed(Models.GiveawayRunning data)
    {
        DateTimeOffset endTime = data.ExpiredDate;
        List<string> parseRole = new List<string>();

        // If required role is available
        if (data.RequiredRole != null)
            parseRole.Add($"Required: <@&{data.RequiredRole}>");

        var embed = new EmbedBuilder()
            .WithTitle("Giveaway Started!")
            .WithColor(Color.Green)
            .WithThumbnailUrl("https://media.discordapp.net/attachments/946050537814655046/948615258078085120/tada.png?width=400&height=394")
            .WithCurrentTimestamp()
            .WithFooter(data.MessageID.ToString(), _client.CurrentUser.GetAvatarUrl())
            .WithDescription(data.GiveawayName)
            .AddField("Note", data.GiveawayNote)
            .AddField("Creator", $"<@!{data.UserID}>", true)
            .AddField("Role", parseRole.Count() == 0 ? "No minimum/required role." : string.Join("\n", parseRole.ToArray()), true)
            .AddField("End Date", $"<t:{endTime.ToUnixTimeSeconds()}:F> which is <t:{endTime.ToUnixTimeSeconds()}:R> from now")
            .AddField("Entries/Winner", $"0 people / {data.WinnerCount} winner", true);
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
        [ModalTextInput("giveaway_winner_count", placeholder: "Ex: 1", initValue: "1", maxLength: 3)]
        public string Winner { get; set; } = default!;

        [InputLabel("Some note?")]
        [ModalTextInput(
            "giveaway_note", TextInputStyle.Paragraph,
            "Spill some note for this giveaway.", 1, 255)]
        public string Note { get; set; } = default!;
    }
}
