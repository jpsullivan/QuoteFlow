using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201508121107)]
    public class M032_AddCustomerIdToQuotes : Migration
    {
        public override void Up()
        {
            Alter.Table("Quotes")
                .AddColumn("CustomerId").AsInt32().NotNullable();
        }

        public override void Down()
        {
            Delete.Column("CustomerId").FromTable("Quotes");
        }
    }
}