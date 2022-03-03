using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;

public class GiveawayRunning
{
    public ulong ID { get; set; } = 0;
    public ulong MessageID { get; set; } = 0;
    public ulong UserMakerID { get; set; } = 0;
    public ulong UserExecutorID { get; set; } = 0;
    public string GiveawayName { get; set; } = null!;
    public DateTime ExpiredDate { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)] public bool IsExpired { get; set; } = false;
    public int WinnerCount { get; set; } = 0;
    public string? Role { get; set; }
    public virtual GiveawayConfig GiveawayConfig { get; set; } = default!;

    public static void Relations(ModelBuilder model)
    {
        model.Entity<GiveawayRunning>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.MessageID).IsRequired();
            entity.Property(e => e.UserMakerID).IsRequired();
            entity.Property(e => e.UserExecutorID).IsRequired();
            entity.Property(e => e.GiveawayName).IsRequired();
            entity.Property(e => e.ExpiredDate).IsRequired();
            entity.Property(e => e.WinnerCount).IsRequired();
            entity.HasOne(e => e.GiveawayConfig).WithMany(e => e.GiveawayRunning);
        });
    }
}
