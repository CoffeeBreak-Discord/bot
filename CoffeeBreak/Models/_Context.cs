using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;
public class DatabaseContext : DbContext
{
    public DbSet<WarnList> WarnList => this.Set<WarnList>();
    public DbSet<GiveawayRunning> GiveawayRunning => this.Set<GiveawayRunning>();
    public DbSet<GiveawayConfig> GiveawayConfig => this.Set<GiveawayConfig>();
    public DbSet<GiveawayParticipant> GiveawayParticipant => this.Set<GiveawayParticipant>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
    {
        var connString = ThirdParty.Database.GenerateConnectionString();
        var serverVersion = ServerVersion.AutoDetect(connString);
        optionBuilder.UseMySql(connectionString: connString, serverVersion);
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        Models.WarnList.Relations(model);
        Models.GiveawayRunning.Relations(model);
        Models.GiveawayConfig.Relations(model);
    }
}
