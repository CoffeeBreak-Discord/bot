using FluentMigrator;

namespace CoffeeBreak.Migrator.MigrationList;
[Migration(20220422)]
public class _20220422_Poll : Migration
{
    public override void Up()
    {
        this.Create.Table("PollRunning")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("GuildID").AsInt64().NotNullable()
            .WithColumn("UserID").AsInt64().NotNullable()
            .WithColumn("MessageID").AsInt64().NotNullable()
            .WithColumn("PollName").AsString().NotNullable()
            .WithColumn("PollChoice").AsCustom("LONGTEXT").NotNullable()
            .WithColumn("ChoiceCount").AsInt32().NotNullable()
            .WithColumn("ExpiredDate").AsDateTime().NotNullable()
            .WithColumn("IsExpired").AsBoolean().WithDefaultValue(false).NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("PollRunning");
    }
}
