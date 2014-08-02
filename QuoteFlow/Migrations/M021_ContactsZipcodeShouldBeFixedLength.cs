using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408021211)]
    public class M021_ContactsZipcodeShouldBeFixedLength : Migration
    {
        public override void Up()
        {
            Alter.Column("Zipcode")
                .OnTable("Contacts")
                .AsFixedLengthAnsiString(5)
                .Nullable();
        }

        public override void Down()
        {
            Alter.Column("Zipcode")
                .OnTable("Contacts")
                .AsString(5)
                .Nullable();
        }
    }
}