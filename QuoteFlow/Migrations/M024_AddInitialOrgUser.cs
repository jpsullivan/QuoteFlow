using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408032051)]
    public class M024_AddInitialOrgUser : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("OrganizationUsers").Row(new {OrganizationId = 1, UserId = 1});
        }

        public override void Down()
        {
            Delete.FromTable("OrganizationUsers").Row(new {OrganizationId = 1});
        }
    }
}