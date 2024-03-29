using FluentMigrator;

namespace CoffeeBreak.Migrator.MigrationList;
[Migration(20220302)]
public class _20220302_Giveaway : Migration
{
    public override void Up()
    {
        this.Create.Table("GiveawayConfig")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("GuildID").AsInt64().NotNullable()
            .WithColumn("ChannelID").AsInt64().NotNullable();

        this.Create.Table("GiveawayRunning")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("GiveawayConfigID").AsInt64().NotNullable().ForeignKey("GiveawayConfig", "ID")
            .WithColumn("MessageID").AsInt64().NotNullable()
            .WithColumn("UserID").AsInt64().NotNullable()
            .WithColumn("GiveawayName").AsString().NotNullable()
            .WithColumn("GiveawayNote").AsString().Nullable()
            .WithColumn("ExpiredDate").AsDateTime().NotNullable()
            .WithColumn("IsExpired").AsBoolean().WithDefaultValue(false).NotNullable()
            .WithColumn("WinnerCount").AsInt16().NotNullable()
            .WithColumn("RequiredRole").AsInt64().Nullable();
    }

    public override void Down()
    {
        this.Delete.Table("GiveawayRunning");
        this.Delete.Table("GiveawayConfig");
    }
}
