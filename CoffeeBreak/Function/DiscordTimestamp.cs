namespace CoffeeBreak.Function;

public class DiscordTimestamp
{
    public long UnixSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
    public DiscordTimestamp(DateTimeOffset dateTimeOffset)
    {
        this.UnixSeconds = dateTimeOffset.ToUnixTimeSeconds();
    }

    public string Format()
    {
        return $"<t:{this.UnixSeconds}>";
    }

    public string ShortTime()
    {
        return this.Format().Replace(">", ":t>");
    }

    public string LongTime()
    {
        return this.Format().Replace(">", ":T>");
    }

    public string ShortDate()
    {
        return this.Format().Replace(">", ":d>");
    }

    public string LongDate()
    {
        return this.Format().Replace(">", ":D>");
    }

    public string ShortDateTime()
    {
        return this.Format().Replace(">", ":f>");
    }

    public string LongDateTime()
    {
        return this.Format().Replace(">", ":F>");
    }

    public string Relative()
    {
        return this.Format().Replace(">", ":R>");
    }
}