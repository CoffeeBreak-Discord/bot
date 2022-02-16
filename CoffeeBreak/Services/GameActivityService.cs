using CoffeeBreak.ThirdParty;
using Discord.WebSocket;

namespace CoffeeBreak.Services;
public class GameActivityService
{
    private readonly DiscordShardedClient _client;
    private readonly string[] _status = Global.Presence.Status;
    private int _statusIndex = 0;
    private int _interval = Global.Presence.Interval;

    public GameActivityService(DiscordShardedClient client)
    {
        _client = client;
        client.ShardReady += this.ShardReady;
    }

    private Task ShardReady(DiscordSocketClient client)
    {
        _client.SetGameAsync(this.SetText());
        Logging.Info("Game activity is started", "GameActivity");
        Interval.SetInterval(() => _client.SetGameAsync(this.SetText()), 10000);
        return Task.CompletedTask;
    }

    private string SetText()
    {
        _statusIndex = _statusIndex >= _status.Length ? 0 : _statusIndex;
        var text = $"/help | {_status[_statusIndex]}";
        _statusIndex++;
        return text;
    }
}
