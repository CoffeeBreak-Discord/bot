using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;
public class GiveawayParticipant
{
    public ulong ID { get; set; } = 0;
    public virtual GiveawayRunning GiveawayRunning { get; set; } = default!;
    public ulong UserID { get; set; } = 0;

    public static void Relations(ModelBuilder model)
    {
        model.Entity<GiveawayParticipant>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.UserID).IsRequired();
            entity.HasOne(e => e.GiveawayRunning).WithMany(e => e.GiveawayParticipant);
        });
    }
}
