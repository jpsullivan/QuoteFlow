using FluentMigrator;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Migrations
{
    [Migration(201408011755)]
    public class M007_CreateCatalogs : Migration
    {
        private const string TableName = "Catalogs";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("Description").AsMaxString().Nullable()
                .WithColumn("OrganizationId").AsInt32().NotNullable()
                .WithColumn("CreatorId").AsInt32().NotNullable()
                .WithColumn("EffectiveDate").AsDateTime().Nullable()
                .WithColumn("ExpirationDate").AsDateTime().Nullable()
                .WithColumn("CreationDate").AsDateTime().NotNullable()
                .WithColumn("LastUpdated").AsDateTime().NotNullable()
                .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(1);
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}