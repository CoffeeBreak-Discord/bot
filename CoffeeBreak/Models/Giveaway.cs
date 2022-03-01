using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;

public class Giveaway
{
    public ulong ID { get; set; } = 0;
    public string HashID { get; set; } = "";
    public ulong GuildID { get; set; } = 0;
    public ulong UserID { get; set; } = 0;
    public ulong ExecutorID { get; set; } = 0;
    public string Name { get; set; } = null!;
    public DateTime Expired { get; set; }
    public int WinnerCount { get; set; } = 0;
    public string Role { get; set; } = "";

    public static void Relations(ModelBuilder model)
    {
        model.Entity<Giveaway>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.HashID).IsRequired();
            entity.Property(e => e.GuildID).IsRequired();
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.ExecutorID).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Expired).IsRequired();
            entity.Property(e => e.WinnerCount).IsRequired();
            entity.Property(e => e.Role).IsRequired();
        });
    }
}
