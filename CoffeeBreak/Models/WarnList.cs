namespace CoffeeBreak.Models;

public class WarnList
{
    public ulong ID { get; set; } = 0;
    public ulong GuildID { get; set; } = 0;
    public ulong UserID { get; set; } = 0;
    public ulong ExecutorID { get; set; } = 0;
    public string Reason { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}