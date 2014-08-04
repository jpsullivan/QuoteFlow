using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408032046)]
    public class M023_AddInitialOrganization : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Organizations").Row(new {OrganizationName = "QuoteFlow", OwnerId = 1, CreatorId = 1});
        }

        public override void Down()
        {
            Delete.FromTable("Organizations").Row(new {OrganizationName = "QuoteFlow"});
        }
    }
}