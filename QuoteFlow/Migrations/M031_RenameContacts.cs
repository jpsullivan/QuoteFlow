using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201508121016)]
    public class M031_RenameContacts : Migration
    {
        public override void Up()
        {
            Rename.Table("Contacts").To("Customers");
        }

        public override void Down()
        {
            Rename.Table("Customers").To("Contacts");
        }
    }
}