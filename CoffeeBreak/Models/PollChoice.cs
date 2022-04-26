using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;

public class PollChoice
{
    public ulong ID { get; set; } = 0;
    public virtual PollRunning PollRunning { get; set; } = default!;
    public string ChoiceValue { get; set; } = null!;

    public static void Relations(ModelBuilder model)
    {
        model.Entity<PollChoice>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ChoiceValue).IsRequired();
            entity.HasOne(e => e.PollRunning).WithMany(e => e.PollChoice);
        });
    }
}
