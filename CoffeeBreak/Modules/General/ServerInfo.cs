using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using CoffeeBreak.Function;

namespace CoffeeBreak.Modules;
public partial class GeneralModule
{
    public enum ChoiceServerInfo
    {
        Normal, Roles, Subscription
    }

    [SlashCommand("serverinfo", "Display server info")]
    public async Task ServerInfo(ChoiceServerInfo menu = ChoiceServerInfo.Normal)
    {
        if (this.Context.Guild == null)
        {
            await this.RespondAsync("You can't use this command because this command only can executed in text channel.");
            return;
        }

        SocketGuild guild = this.Context.Guild;
        EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithAuthor(guild.Name)
            .WithThumbnailUrl(guild.IconUrl)
            .WithCurrentTimestamp()
            .WithFooter($"Requested by {this.Context.User}");

        switch (menu)
        {
            case ChoiceServerInfo.Normal: this.GetServerInfo(ref embed, guild); break;
            case ChoiceServerInfo.Roles: this.GetRoleServerInfo(ref embed, guild); break;
            case ChoiceServerInfo.Subscription: embed.WithDescription("Not implemented, sorry"); break;
        }

        await this.RespondAsync(embed: embed.Build());
    }

    private void GetServerInfo(ref EmbedBuilder embed, SocketGuild guild)
    {
        DiscordTimestamp createdTimestamp = new DiscordTimestamp(guild.CreatedAt);
        embed
            .AddField("ID", guild.Id, true)
            .AddField("Verification Level", guild.VerificationLevel.ToString(), true)
            .AddField("Shard ID", _client.GetShardIdFor(guild), true)
            .AddField("Members", guild.MemberCount, true)
            .AddField(
                $"Channels [{guild.Channels.Count}]",
                $"Category: {guild.CategoryChannels.Count}\nText: {guild.TextChannels.Count}\nVoice: {guild.StageChannels.Count + guild.VoiceChannels.Count}",
                true)
            .AddField("Server Owner", $"{_client.Rest.GetUserAsync(guild.OwnerId).GetAwaiter().GetResult().Mention}", true)
            .AddField("Created", $"{createdTimestamp.longDateTime()} ({createdTimestamp.relative()})", true)
            .AddField("Server Boost", $"Level: {guild.PremiumTier}\nBoost Count: {guild.PremiumSubscriptionCount}")
            .AddField("Roles", "Use `/serverinfo menu:Roles` for more information.")
            .AddField("Subscription", "Not implemented.");

        if (guild.BannerUrl != null)
            embed.WithImageUrl(guild.BannerUrl);
    }

    private void GetRoleServerInfo(ref EmbedBuilder embed, SocketGuild guild)
    {
        var roles = guild.Roles.Select(x => x.Mention);
        embed
            .WithDescription(string.Join(", ", roles.ToArray()))
            .WithTitle($"Roles [{roles.Count()}]:");
    }
}