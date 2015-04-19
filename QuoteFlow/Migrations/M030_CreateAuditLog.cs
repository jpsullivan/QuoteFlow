using FluentMigrator;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Migrations
{
    [Migration(201504171615)]
    public class M030_CreateAuditLog : Migration
    {
        private const string TableName = "AuditLog";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Category").AsInt32().NotNullable()
                .WithColumn("Event").AsInt32().NotNullable()
                .WithColumn("UserId").AsInt32().NotNullable()
                .WithColumn("Details").AsMaxString().Nullable()
                .WithColumn("CreatedUtc").AsDateTime().Nullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_AuditLog_Users_UserId")
                .FromTable(TableName).ForeignColumn("UserId")
                .ToTable("Users").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_AuditLog_Users_UserId").OnTable(TableName);
            Delete.Table(TableName);
        }
    }
}