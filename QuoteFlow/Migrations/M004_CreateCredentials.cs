using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011738)]
    public class M004_CreateCredentials : Migration
    {
        private const string TableName = "Credentials";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().NotNullable()
                .WithColumn("Type").AsString(64).NotNullable()
                .WithColumn("Value").AsString(256).NotNullable()
                .WithColumn("Ident").AsString(256).Nullable();

            Create.ForeignKey("FK_Credentials_Credentials")
                .FromTable(TableName).ForeignColumn("Id")
                .ToTable(TableName).PrimaryColumn("Id");

            Create.ForeignKey("FK_Credentials_Users_UserId")
                .FromTable(TableName).ForeignColumn("UserId")
                .ToTable("Users").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_Credentials_Users_UserId");
            Delete.ForeignKey("FK_Credentials_Credentials");
            Delete.Table(TableName);
        }
    }
}