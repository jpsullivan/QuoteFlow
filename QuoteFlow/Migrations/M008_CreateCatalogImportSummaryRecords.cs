using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011807)]
    public class M008_CreateCatalogImportSummaryRecords : Migration
    {
        private const string TableName = "CatalogImportSummaryRecords";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("CatalogId").AsInt32().NotNullable()
                .WithColumn("Result").AsString(15).NotNullable()
                .WithColumn("RowId").AsInt32().NotNullable()
                .WithColumn("AssetId").AsInt32().Nullable()
                .WithColumn("Reason").AsString(350).Nullable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}