using FluentMigrator;

namespace CoffeeBreak.Migrator.MigrationList;
[Migration(20220328)]
public class _20220328_StageConfig : Migration
{
    public override void Up()
    {
        this.Create.Table("StageConfig")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("GuildID").AsInt64().NotNullable()
            .WithColumn("RoleID").AsInt64().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("StageConfig");
    }
}
