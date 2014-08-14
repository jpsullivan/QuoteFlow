using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408141858)]
    public class M026_AddTokenToManufacturerLogos : Migration
    {
        public override void Up()
        {
            Alter.Table("ManufacturerLogos")
                .AddColumn("Token").AsGuid().NotNullable();
        }

        public override void Down()
        {
            Delete.Column("Token").FromTable("ManufacturerLogos");
        }
    }
}