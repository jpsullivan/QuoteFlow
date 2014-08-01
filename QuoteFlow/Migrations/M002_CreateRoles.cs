using FluentMigrator;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Migrations
{
    [Migration(201408011714)]
    public class M002_CreateRoles : Migration
    {
        private const string TableName = "Roles";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsMaxString().Nullable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}