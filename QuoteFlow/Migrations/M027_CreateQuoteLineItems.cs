using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201502221601)]
    public class M027_CreateQuoteLineItems : Migration
    {
        private const string TableName = "QuoteLineItems";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("QuoteId").AsInt32().NotNullable()
                .WithColumn("AssetId").AsInt32().NotNullable()
                .WithColumn("Quantity").AsInt32().NotNullable()
                .WithColumn("CreatedUtc").AsDateTime().Nullable();

            Create.ForeignKey("FK_QuoteLineItems_Quotes_QuoteId")
                .FromTable(TableName).ForeignColumn("QuoteId")
                .ToTable("Quotes").PrimaryColumn("Id");

            Create.ForeignKey("FK_QuoteLineItems_Assets_AssetId")
                .FromTable(TableName).ForeignColumn("AssetId")
                .ToTable("Assets").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_QuoteLineItems_Assets_AssetId");
            Delete.ForeignKey("FK_QuoteLineItems_Quotes_QuoteId");
            Delete.Table(TableName);
        }
    }
}