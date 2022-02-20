using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;

public class WarnList
{
    public ulong ID { get; set; } = 0;
    public ulong GuildID { get; set; } = 0;
    public ulong UserID { get; set; } = 0;
    public ulong ExecutorID { get; set; } = 0;
    public string Reason { get; set; } = null!;
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)] public DateTime Timestamp { get; set; }

    public static void Relations(ModelBuilder model)
    {
        model.Entity<WarnList>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.GuildID).IsRequired();
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.ExecutorID).IsRequired();
            entity.Property(e => e.Reason).IsRequired();
        });
    }
}
