using CoffeeBreak.Function;
using CoffeeBreak.ThirdParty;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Modules;
public partial class RaffleGiveawayModule
{
    // If user spawning giveaway, first of all user will teleport to
    // this interaction to parse the menu value.
    [ComponentInteraction("select_menu_giveaway")]
    public async Task GiveawaySelectRoleResponse(string[] selectedMenu)
    {
        string menu = selectedMenu[0];
        if (menu == "role_none")
        {
            await this.Context.Interaction.RespondWithModalAsync<GiveawayManager.GiveawayModal>("modal_giveaway:none;0");
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
        => await this.Context.Interaction.RespondWithModalAsync<GiveawayManager.GiveawayModal>($"modal_giveaway:required;{selectedMenu[0] ?? "0"}");

    [ModalInteraction("modal_giveaway:*;*")]
    public async Task GiveawayModalResponse(string mode, string roleID, GiveawayManager.GiveawayModal modal)
    {
        Models.GiveawayRunning data = new Models.GiveawayRunning();
        data.GiveawayName = modal.GiveawayName;
        data.GiveawayNote = modal.Note;
        data.UserID = this.Context.User.Id;
        data.WinnerCount = int.Parse(modal.Winner);
        data.ExpiredDate = new HumanizeDuration(modal.Duration).ToDateTime();

        // If expired date < 5 seconds, increase to 30 seconds.
        // We need time to make sure database is completed before expiration.
        // And u know, "human thingy"
        TimeSpan expDiff = data.ExpiredDate - DateTime.Now;
        if (Math.Floor(expDiff.TotalSeconds) < 5) data.ExpiredDate = new HumanizeDuration("30s").ToDateTime();

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
        data.GiveawayConfig = channelConf;

        // Save to database
        await _db.GiveawayRunning.AddAsync(data);
        await _db.SaveChangesAsync();

        // Set button to giveaway message
        var comp = new ComponentBuilder()
            .WithButton("Turn me in! 🎉", $"button_giveaway_toggle:{data.ID}", ButtonStyle.Primary)
            .Build();
        await message.ModifyAsync(Message =>
        {
            Message.Components = comp;
            Message.Embed = GiveawayManager.GenerateEmbed(data, _client);
            Message.Content = null;
        });

        // If the giveaway less than State.Giveaway.MinuteInterval, proceed to cache
        TimeSpan ts = data.ExpiredDate - DateTime.Now;
        int minDiff = (int) Math.Floor(ts.TotalMinutes);
        int interval = Global.State.Giveaway.MinuteInterval;
        if (interval >= minDiff)
        {
            Logging.Info($"Add ID:{data.ID} to State.Giveaway.GiveawayActive because the Giveaway less than {interval} minutes.", "GAPool");
            Global.State.Giveaway.GiveawayActive.Add(data.ID, data.ExpiredDate);
        }

        await this.RespondAsync(
            $"Giveaway successfully created! Check <#{data.GiveawayConfig.ChannelID}> to see your giveaway.\n"
            + $"If you want to edit some giveaway, you can use `/giveaway modify id:{data.ID} [param]`.",
            ephemeral: true);
    }

    [ComponentInteraction("button_giveaway_toggle:*")]
    public async Task GiveawayButtonToggleResponse(string ids)
    {
        // Check giveaway is running or not
        ulong id = ulong.Parse(ids);
        var data = await _db.GiveawayRunning.Include(m => m.GiveawayConfig).Where(x => x.ID == id).FirstOrDefaultAsync();
        if (data == null)
        {
            await this.RespondAsync(
                "Something error when joining the giveaway:\n```No giveaway running in this server.```",
                ephemeral: true);
            return;
        }
        if (data.IsExpired)
        {
            await this.RespondAsync(
                "Something error when joining the giveaway:\n```This giveaway has completed.```",
                ephemeral: true);
            return;
        }

        // Check if giveaway REQUIRED_ROLE
        // Special cast for checking numeric data type
        if (data.RequiredRole is ulong roleID)
        {
            SocketGuildUser? user = this.Context.Guild.GetUser(this.Context.User.Id);
            bool isAdmin = user.Roles.Where(x => x.Permissions.Has(GuildPermission.Administrator)).Count() > 0;
            bool isHaveRole = user.Roles.Where(x => x.Id == roleID).Count() > 0;
            if (!isHaveRole && !isAdmin)
            {
                await this.RespondAsync(
                    "Something error when joining the giveaway:\n```You didn't meet requirement: No required role exist.```",
                    ephemeral: true);
                return;
            }
        }

        // Fetch channel and message
        var channel = this.Context.Guild.GetTextChannel(data.GiveawayConfig.ChannelID);
        if (channel == null) return;
        var message = await channel.GetMessageAsync(data.MessageID);
        if (message == null) return;

        // If user has joined, unregister from giveaway, nevertheless add to database
        var entity = await _db.GiveawayParticipant.FirstOrDefaultAsync(
            x => x.UserID == this.Context.User.Id && x.GiveawayRunning == data);
        bool isParticipated;
        if (entity != null)
        {
            _db.GiveawayParticipant.Remove(entity);
            isParticipated = false;
        }
        else
        {
            await _db.GiveawayParticipant.AddAsync(new Models.GiveawayParticipant
            {
                UserID = this.Context.User.Id,
                GiveawayRunning = data
            });
            isParticipated = true;
        }
        await _db.SaveChangesAsync();

        // Recheck entries
        ulong entries = (ulong) await _db.GiveawayParticipant
            .Include(m => m.GiveawayRunning)
            .Where(x => x.GiveawayRunning == data)
            .CountAsync();
        var embed = message.Embeds.First().ToEmbedBuilder();
        int embedIndex = embed.Fields.FindIndex(x => x.Name == "Entries/Winner");
        embed.Fields[embedIndex].Value = $"{entries} people{(entries > 1 ? "s" : "")} / {data.WinnerCount} winner";
        await channel.ModifyMessageAsync(message.Id, Message => Message.Embed = embed.Build());
        
        await this.RespondAsync(
            isParticipated ? $"You participated to **{embed.Description}** giveaway." : $"You cancelled participation to **{embed.Description}** giveaway.",
            ephemeral: true);
    }

    [ComponentInteraction("button_giveaway_reroll:*")]
    public async Task GiveawayRerollButtonResponse(string ids)
    {
        ulong id = ulong.Parse(ids);
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

        var channel = this.Context.Guild.GetChannel(data.GiveawayConfig.ChannelID) as SocketTextChannel;
        if (channel == null) return;
        var message = await channel.GetMessageAsync(data.MessageID);
        if (message == null) return;

        // If giveaway more than one days (fixed), remove the button
        TimeSpan ts = DateTime.Now - data.ExpiredDate.AddDays(1);
        if (Math.Ceiling(ts.TotalDays) > 0)
        {
            var button = new ComponentBuilder()
                .WithButton("Reroll! 🔂", $"button_giveaway_reroll:{data!.ID}", ButtonStyle.Danger, disabled: true)
                .Build();
            await channel.ModifyMessageAsync(message.Id, Message => Message.Components = button);
            await this.RespondAsync(
                "This giveaway is expired, please make new giveaway using `/giveaway start` command.",
                ephemeral: true);
            return;
        }

        await GiveawayManager.StopGiveawayAsync(this.Context.Guild, channel, message, _db, data.ID);
        await this.RespondAsync("Giveaway successfully rerolled!", ephemeral: true);
    }
}
