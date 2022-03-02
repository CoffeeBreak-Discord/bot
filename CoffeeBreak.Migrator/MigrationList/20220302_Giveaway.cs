using FluentMigrator;

namespace CoffeeBreak.Migrator.MigrationList;
[Migration(20220302)]
public class _20220302_Giveaway : Migration
{
    public override void Up()
    {
        this.Create.Table("GiveawayRunning")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("HashID").AsString().Unique().NotNullable()
            .WithColumn("GuildID").AsInt64().NotNullable()
            .WithColumn("MessageID").AsInt64().NotNullable()
            .WithColumn("UserMakerID").AsInt64().NotNullable()
            .WithColumn("UserExecutorID").AsInt64().NotNullable()
            .WithColumn("GiveawayName").AsString().NotNullable()
            .WithColumn("ExpiredDate").AsDateTime().NotNullable()
            .WithColumn("IsExpired").AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn("WinnerCount").AsInt16().NotNullable()
            .WithColumn("Role").AsString().NotNullable();
        
        this.Create.Table("GiveawayConfig")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("GuildID").AsInt64().NotNullable()
            .WithColumn("ChannelID").AsInt64().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("Giveaway");
    }
}
