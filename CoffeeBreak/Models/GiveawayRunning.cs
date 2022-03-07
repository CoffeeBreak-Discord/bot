using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;

public class GiveawayRunning
{
    public ulong ID { get; set; } = 0;
    public virtual GiveawayConfig GiveawayConfig { get; set; } = default!;
    public ulong MessageID { get; set; } = 0;
    public ulong UserID { get; set; } = 0;
    public string GiveawayName { get; set; } = null!;
    public string? GiveawayNote { get; set; }
    public DateTime ExpiredDate { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)] public bool IsExpired { get; set; } = false;
    public int WinnerCount { get; set; } = 0;
    public ulong? RequiredRole { get; set; }

    public static void Relations(ModelBuilder model)
    {
        model.Entity<GiveawayRunning>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.MessageID).IsRequired();
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.GiveawayName).IsRequired();
            entity.Property(e => e.ExpiredDate).IsRequired();
            entity.Property(e => e.WinnerCount).IsRequired();
            entity.HasOne(e => e.GiveawayConfig).WithMany(e => e.GiveawayRunning);
        });
    }
}
