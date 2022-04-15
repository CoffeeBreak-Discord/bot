using Discord;

namespace CoffeeBreak.Function;
public class SetupManager
{
    private EmbedBuilder _embed;
    private string _title;
    private bool _status;

    public SetupManager(string title, string? thumbnailUrl)
    {
        _title = title;
        _embed = new EmbedBuilder()
            .WithColor(Global.BotColors.Randomize().IntCode)
            .WithTitle($"Setup {_title} Module")
            .WithCurrentTimestamp();

        if (thumbnailUrl != null) _embed.WithThumbnailUrl(thumbnailUrl);
    }

    public void SetStatus(bool status) => _status = status;

    public Embed Build()
    {
        // Add status to field
        _embed.AddField($"{_title} Module Status", _status ? "Active" : "Not Active");
        return _embed.Build();
    }
}
