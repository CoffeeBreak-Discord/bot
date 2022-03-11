namespace CoffeeBreak.Function;

public class DiscordTimestamp
{
    public long unixSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
    public DiscordTimestamp(DateTimeOffset dateTimeOffset)
    {
        this.unixSeconds = dateTimeOffset.ToUnixTimeSeconds();
    }

    public string format()
    {
        return $"<t:{this.unixSeconds}>";
    }

    public string shortTime()
    {
        return this.format().Replace(">", ":t>");
    }

    public string longTime()
    {
        return this.format().Replace(">", ":T>");
    }

    public string shortDate()
    {
        return this.format().Replace(">", ":d>");
    }

    public string longDate()
    {
        return this.format().Replace(">", ":D>");
    }

    public string shortDateTime()
    {
        return this.format().Replace(">", ":f>");
    }

    public string longDateTime()
    {
        return this.format().Replace(">", ":F>");
    }

    public string relative()
    {
        return this.format().Replace(">", ":R>");
    }
}