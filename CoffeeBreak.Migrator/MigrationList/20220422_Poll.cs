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
            .WithColumn("ChoiceCount").AsInt32().NotNullable()
            .WithColumn("ExpiredDate").AsDateTime().NotNullable()
            .WithColumn("IsExpired").AsBoolean().WithDefaultValue(false).NotNullable();

        this.Create.Table("PollChoice")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("PollRunningID").AsInt64().NotNullable().ForeignKey("PollRunning", "ID")
            .WithColumn("ChoiceValue").AsString().NotNullable();

        this.Create.Table("PollParticipant")
            .WithColumn("ID").AsInt64().PrimaryKey().Identity().NotNullable()
            .WithColumn("PollChoiceID").AsInt64().NotNullable().ForeignKey("PollChoice", "ID")
            .WithColumn("PollRunningID").AsInt64().NotNullable().ForeignKey("PollRunning", "ID")
            .WithColumn("UserID").AsInt64().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("PollParticipant");
        this.Delete.Table("PollChoice");
        this.Delete.Table("PollRunning");
    }
}
