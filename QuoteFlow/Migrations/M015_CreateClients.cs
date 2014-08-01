using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011846)]
    public class M015_CreateClients : Migration
    {
        private const string TableName = "Clients";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("Name").AsString(150).NotNullable()
                .WithColumn("OrganizationId").AsInt32().NotNullable()
                .WithColumn("CreationDate").AsDateTime().NotNullable()
                .WithColumn("LastUpdated").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_Clients_Organizations_Id");
            Delete.Table(TableName);
        }
    }
}