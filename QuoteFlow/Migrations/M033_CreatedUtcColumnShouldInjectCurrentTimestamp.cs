using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201511300258)]
    public class M033_CreatedUtcColumnShouldInjectCurrentTimestamp : Migration
    {
        public override void Up()
        {
            Alter.Table("QuoteLineItems").AlterColumn("CreatedUtc").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
        }

        public override void Down()
        {
            Alter.Table("QuoteLineItems").AlterColumn("CreatedUtc").AsDateTime().NotNullable();
        }
    }
}