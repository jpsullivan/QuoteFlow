using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408141628)]
    public class M025_CreateManufacturerLogos : Migration
    {
        private const string TableName = "ManufacturerLogos";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("ManufacturerId").AsInt32().NotNullable()
                .WithColumn("Url").AsString(400).NotNullable()
                .WithColumn("CreationDate").AsDateTime().NotNullable()
                .WithColumn("LastUpdated").AsDateTime().NotNullable();

            Create.ForeignKey("FK_ManufacturerLogos_Manufacturers_Id")
                .FromTable(TableName).ForeignColumn("ManufacturerId")
                .ToTable("Manufacturers").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_ManufacturerLogos_Manufacturers_Id");
            Delete.Table(TableName);
        }
    }
}