using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408021216)]
    public class M022_CreateELMAH : Migration
    {
        public override void Up()
        {
            Execute.EmbeddedScript("Elmah.SqlServer.sql");
        }

        public override void Down()
        {
            Delete.Table("ELMAH_Error");
        }
    }
}