using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201502231145)]
    public class M028_CreateQuoteStatus : Migration
    {
        private const string TableName = "QuoteStatus";

        public override void Up()
        {
            // give Organiations a primary key on the Id column, removing the existing one
            Delete.PrimaryKey("PK_Organizations").FromTable("Organizations");
            Create.PrimaryKey("PK_Organizations_Id").OnTable("Organizations").Column("Id");

            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("OrganizationId").AsInt32().NotNullable()
                .WithColumn("OrderNum").AsInt32().NotNullable()
                .WithColumn("CreatedUtc").AsDateTime().Nullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_QuoteStatus_Organizations_OrganizationId")
                .FromTable(TableName).ForeignColumn("OrganizationId")
                .ToTable("Organizations").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_QuoteStatus_Organizations_OrganizationId");

            // reset the primary keys on the orgs table
            Delete.PrimaryKey("PK_Organizations_Id").FromTable("Organizations");
            Create.PrimaryKey("PK_Organizations").OnTable("Organizations").Column("OrganizationName");

            Delete.Table(TableName);
        }
    }
}