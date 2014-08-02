using FluentMigrator;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Migrations
{
    [Migration(201408021158)]
    public class M019_CreateQuotes : Migration
    {
        private const string TableName = "Quotes";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("Description").AsMaxString().Nullable()
                .WithColumn("Status").AsInt32().NotNullable()
                .WithColumn("Responded").AsBoolean().NotNullable().WithDefaultValue(0)
                .WithColumn("CreatorId").AsInt32().NotNullable()
                .WithColumn("OrganizationId").AsInt32().NotNullable()
                .WithColumn("CreationDate").AsDateTime().NotNullable()
                .WithColumn("LastUpdated").AsDateTime().NotNullable()
                .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(1);

            Create.ForeignKey("FK_Quotes_Users_Id")
                .FromTable(TableName).ForeignColumn("CreatorId")
                .ToTable("Users").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_Quotes_Users_Id");
            Delete.Table(TableName);
        }
    }
}