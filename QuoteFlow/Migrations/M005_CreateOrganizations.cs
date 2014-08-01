using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011742)]
    public class M005_CreateOrganizations : Migration
    {
        private const string TableName = "Organizations";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity()
                .WithColumn("OrganizationName").AsString(100).NotNullable().PrimaryKey()
                .WithColumn("OwnerId").AsInt32().NotNullable()
                .WithColumn("CreatorId").AsInt32().NotNullable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}