using FluentMigrator;

namespace CoffeeBreak.Database.Migrator.MigrationList;
[Migration(20220207)]
public class _20220207_ClassicModeration : Migration
{
    public override void Up()
    {
        this.Create.Table("WarnList")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("GuildID").AsInt64().NotNullable()
            .WithColumn("UserID").AsInt64().NotNullable()
            .WithColumn("ExecutorID").AsInt64().NotNullable()
            .WithColumn("Reason").AsString().NotNullable()
            .WithColumn("Timestamp").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down()
    {
        this.Delete.Table("WarnList");
    }
}
