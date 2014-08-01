using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011752)]
    public class M006_CreateOrganizationUsers : Migration
    {
        private const string TableName = "OrganizationUsers";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("OrganizationId").AsInt32().NotNullable()
                .WithColumn("UserId").AsInt32().NotNullable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}