using Discord;
using Discord.Interactions;

namespace CoffeeBreak.Function;
public class SetupEmbedBuilder
{
    private EmbedBuilder _embed;
    private string _title;
    private bool _status = true;
    private Dictionary<string, string> _field = new Dictionary<string, string>();
    private ShardedInteractionContext _ctx;

    public SetupEmbedBuilder(ShardedInteractionContext context, string title, string? thumbnailUrl = null)
    {
        _ctx = context;
        _title = title;
        _embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithTitle($"Setup {_title} Module")
            .WithCurrentTimestamp()
            .WithFooter($"Requested by {_ctx.User}");

        if (thumbnailUrl != null) _embed.WithThumbnailUrl(thumbnailUrl);
    }

    public void SetStatus(bool status) => _status = status;

    public void AddField(string commandName, string description) => _field.Add($"/{_title.ToLower()} {commandName}", description);

    public Embed Build()
    {
        // Add status to field
        _embed.AddField($"{_title} Module Status", _status ? "Active" : "Not Active");

        // If status is false, add warning description
        if (!_status)
            _embed.WithDescription("This module is not ready enough to be used right now. Please check requirement below to make this module ready in this guild.");

        // Loop the data
        foreach (var field in _field) _embed.AddField(field.Key, field.Value);
        return _embed.Build();
    }
}
