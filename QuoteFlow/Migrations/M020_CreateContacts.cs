using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408021202)]
    public class M020_CreateContacts : Migration
    {
        private const string TableName = "Contacts";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("OrganizationId").AsInt32().NotNullable()
                .WithColumn("FirstName").AsString(50).NotNullable()
                .WithColumn("LastName").AsString(50).NotNullable()
                .WithColumn("EmailAddress").AsString(255).Nullable()
                .WithColumn("Phone").AsString(15).Nullable()
                .WithColumn("Organization").AsString(150).Nullable()
                .WithColumn("Title").AsString(50).Nullable()
                .WithColumn("Address1").AsString(200).Nullable()
                .WithColumn("Address2").AsString(200).Nullable()
                .WithColumn("City").AsAnsiString(50).Nullable()
                .WithColumn("State").AsAnsiString(10).Nullable()
                .WithColumn("Country").AsAnsiString(50).Nullable()
                .WithColumn("Zipcode").AsString(5).Nullable()
                .WithColumn("CreationDate").AsDateTime().NotNullable()
                .WithColumn("LastUpdated").AsDateTime().NotNullable()
                .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(1);
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}