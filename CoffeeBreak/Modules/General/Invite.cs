using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    [SlashCommand("invite", "Invite this bot to your server.")]
    public async Task InviteCommandAsync()
    {
        var embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithTitle("Invite the bot!")
            .WithDescription("Invite the bot to your server by clicking the button below.")
            .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl())
            .Build();
        var component = new ComponentBuilder()
            .WithButton("Invite Bot", style: ButtonStyle.Link, url: Global.Bot.LinkInvite)
            .WithButton("Discord Support Server", style: ButtonStyle.Link, url: Global.Bot.DiscordServer)
            .Build();
        
        await this.RespondAsync(embed: embed, components: component, ephemeral: true);
    }
}
