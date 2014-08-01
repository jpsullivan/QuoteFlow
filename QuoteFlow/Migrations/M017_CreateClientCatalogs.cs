using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011852)]
    public class M017_CreateClientCatalogs : Migration
    {
        private const string TableName = "ClientCatalogs";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("ClientId").AsInt32().NotNullable()
                .WithColumn("CatalogId").AsInt32().NotNullable();

            Create.ForeignKey("FK_ClientCatalogs_Clients_Id")
                .FromTable(TableName).ForeignColumn("ClientId")
                .ToTable("Clients").PrimaryColumn("Id");

            Create.ForeignKey("FK_ClientCatalogs_Catalogs_Id")
                .FromTable(TableName).ForeignColumn("CatalogId")
                .ToTable("Catalogs").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_ClientCatalogs_Catalogs_Id");
            Delete.ForeignKey("FK_ClientCatalogs_Clients_Id");
            Delete.Table(TableName);
        }
    }
}