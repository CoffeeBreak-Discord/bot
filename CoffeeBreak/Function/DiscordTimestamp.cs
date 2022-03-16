namespace CoffeeBreak.Function;
public class DiscordTimestamp
{
    public long UnixSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
    public DiscordTimestamp(DateTimeOffset dateTimeOffset)
    {
        this.UnixSeconds = dateTimeOffset.ToUnixTimeSeconds();
    }

    public string Format() => $"<t:{this.UnixSeconds}>";

    public string ShortTime() => this.Format().Replace(">", ":t>");

    public string LongTime() => this.Format().Replace(">", ":T>");

    public string ShortDate() => this.Format().Replace(">", ":d>");

    public string LongDate() => this.Format().Replace(">", ":D>");

    public string ShortDateTime() => this.Format().Replace(">", ":f>");

    public string LongDateTime() => this.Format().Replace(">", ":F>");

    public string Relative() => this.Format().Replace(">", ":R>");
}