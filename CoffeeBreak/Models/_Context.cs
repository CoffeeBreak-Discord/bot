using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;
public class DatabaseContext : DbContext
{
    public DbSet<WarnList> WarnList => this.Set<WarnList>();
    public DbSet<GiveawayRunning> GiveawayRunning => this.Set<GiveawayRunning>();
    public DbSet<GiveawayConfig> GiveawayConfig => this.Set<GiveawayConfig>();
    public DbSet<GiveawayParticipant> GiveawayParticipant => this.Set<GiveawayParticipant>();
    public DbSet<StageConfig> StageConfig => this.Set<StageConfig>();
    public DbSet<PollRunning> PollRunning => this.Set<PollRunning>();
    public DbSet<PollChoice> PollChoice => this.Set<PollChoice>();
    public DbSet<PollParticipant> PollParticipant => this.Set<PollParticipant>();

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
        Models.GiveawayParticipant.Relations(model);
        Models.StageConfig.Relations(model);
        Models.PollRunning.Relations(model);
        Models.PollChoice.Relations(model);
        Models.PollParticipant.Relations(model);
    }
}
