using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011850)]
    public class M016_CreateClientUsers : Migration
    {
        private const string TableName = "ClientUsers";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("CatalogId").AsInt32().NotNullable()
                .WithColumn("UserId").AsInt32().NotNullable();

            Create.ForeignKey("FK_ClientUsers_Catalogs_Id")
                .FromTable(TableName).ForeignColumn("CatalogId")
                .ToTable("Catalogs").PrimaryColumn("Id");

            Create.ForeignKey("FK_ClientUsers_Users_Id")
                .FromTable(TableName).ForeignColumn("UserId")
                .ToTable("Users").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_ClientUsers_Users_Id");
            Delete.ForeignKey("FK_ClientUsers_Catalogs_Id");
            Delete.Table(TableName);
        }
    }
}