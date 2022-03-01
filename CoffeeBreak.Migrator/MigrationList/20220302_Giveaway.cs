using FluentMigrator;

namespace CoffeeBreak.Migrator.MigrationList;
[Migration(20220302)]
public class _20220302_Giveaway : Migration
{
    public override void Up()
    {
        this.Create.Table("Giveaway")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("HashID").AsString().Unique().NotNullable()
            .WithColumn("GuildID").AsInt64().NotNullable()
            .WithColumn("UserID").AsInt64().NotNullable()
            .WithColumn("ExecutorID").AsInt64().NotNullable()
            .WithColumn("Name").AsString().NotNullable()
            .WithColumn("Expired").AsDateTime().NotNullable()
            .WithColumn("WinnerCount").AsInt16().NotNullable()
            .WithColumn("Role").AsString().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("Giveaway");
    }
}
