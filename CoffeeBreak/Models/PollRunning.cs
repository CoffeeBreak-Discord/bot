using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;

public class PollRunning
{
    public ulong ID { get; set; } = 0;
    public ulong GuildID { get; set; } = 0;
    public ulong MessageID { get; set; } = 0;
    public ulong UserID { get; set; } = 0;
    public string PollName { get; set; } = null!;
    public string PollChoice { get; set; } = null!;
    public DateTime ExpiredDate { get; set; }
    public bool IsExpired { get; set; }

    public static void Relations(ModelBuilder model)
    {
        model.Entity<PollRunning>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.GuildID).IsRequired();
            entity.Property(e => e.MessageID).IsRequired();
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.PollName).IsRequired();
            entity.Property(e => e.PollChoice).IsRequired();
            entity.Property(e => e.ExpiredDate).IsRequired();
        });
    }
}
