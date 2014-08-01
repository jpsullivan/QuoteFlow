using FluentMigrator;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Migrations
{
    [Migration(201408011810)]
    public class M009_CreateAssets : Migration
    {
        private const string TableName = "Assets";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("Name").AsString(250).NotNullable()
                .WithColumn("SKU").AsString(150).NotNullable()
                .WithColumn("Type").AsString(50).NotNullable()
                .WithColumn("Description").AsMaxString().Nullable()
                .WithColumn("Cost").AsCurrency().NotNullable()
                .WithColumn("Markup").AsDecimal(9, 2).Nullable()
                .WithColumn("ManufacturerId").AsInt32().Nullable()
                .WithColumn("CatalogId").AsInt32().NotNullable()
                .WithColumn("CreatorId").AsInt32().NotNullable()
                .WithColumn("LastUpdated").AsDateTime().NotNullable()
                .WithColumn("CreationDate").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}