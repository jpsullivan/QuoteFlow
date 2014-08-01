using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011727)]
    public class M003_CreateUserRoles : Migration
    {
        private const string TableName = "UserRoles";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("UserId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("RoleId").AsInt32().NotNullable().PrimaryKey();

            Create.ForeignKey("FK_UserRoles_Roles_RoleId")
                .FromTable(TableName).ForeignColumn("RoleId")
                .ToTable("Roles").PrimaryColumn("Id");

            Create.ForeignKey("FK_UserRoles_Users_UserId")
                .FromTable(TableName).ForeignColumn("UserId")
                .ToTable("Users").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_UserRoles_Users_UserId");
            Delete.ForeignKey("FK_UserRoles_Roles_RoleId");
            Delete.Table(TableName);
        }
    }
}