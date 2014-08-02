using FluentMigrator;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Migrations
{
    [Migration(201408012157)]
    public class M018_CreateManufacturers : Migration
    {
        private const string TableName = "Manufacturers";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("Description").AsMaxString().Nullable()
                .WithColumn("OrganizationId").AsInt32().NotNullable()
                .WithColumn("CreationDate").AsDateTime().NotNullable()
                .WithColumn("LastUpdated").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}