using FluentMigrator;

namespace CoffeeBreak.Migrator.MigrationList;
[Migration(20220501)]
public class _20220501_PollHotfix : Migration
{
    public override void Up()
    {
        this.Alter.Table("PollRunning")
            .AlterColumn("IsOptionsRequired").AsBoolean().NotNullable().WithDefaultValue(true);
    }

    public override void Down()
    {
        this.Alter.Table("PollRunning")
            .AlterColumn("IsOptionsRequired").AsBoolean().NotNullable().WithDefaultValue(false);
    }
}
