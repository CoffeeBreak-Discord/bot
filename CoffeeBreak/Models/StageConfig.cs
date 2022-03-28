using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;
public class StageConfig
{
    public ulong ID { get; set; } = 0;
    public ulong GuildID { get; set; } = 0;
    public ulong RoleID { get; set; } = 0;

    public static void Relations(ModelBuilder model)
    {
        model.Entity<StageConfig>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.GuildID).IsRequired();
            entity.Property(e => e.RoleID).IsRequired();
        });
    }
}
