using Microsoft.EntityFrameworkCore;

namespace CoffeeBreak.Models;
public class Context : DbContext
{
    public DbSet<WarnList> WarnList => this.Set<WarnList>();

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
    }
}
