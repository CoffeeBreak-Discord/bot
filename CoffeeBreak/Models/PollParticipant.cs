using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;

public class PollParticipant
{
    public ulong ID { get; set; } = 0;
    public virtual PollChoice PollChoice { get; set; } = default!;
    public virtual PollRunning PollRunning { get; set; } = default!;
    public ulong UserID { get; set; } = 0;

    public static void Relations(ModelBuilder model)
    {
        model.Entity<PollParticipant>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.UserID).IsRequired();
            entity.HasOne(e => e.PollRunning).WithMany(e => e.PollParticipant);
            entity.HasOne(e => e.PollChoice).WithMany(e => e.PollParticipant);
        });
    }
}
