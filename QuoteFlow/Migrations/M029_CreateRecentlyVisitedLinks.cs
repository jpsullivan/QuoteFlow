using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201504171108)]
    public class M029_CreateRecentlyVisitedLinks : Migration
    {
        private const string TableName = "RecentlyVisitedLinks";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().NotNullable()
                .WithColumn("PageType").AsString(25).NotNullable()
                .WithColumn("PageId").AsInt32().NotNullable()
                .WithColumn("PageName").AsString(256).NotNullable()
                .WithColumn("VisitedUtc").AsDateTime().Nullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_RecentlyVisitedLinks_Users_UserId")
                .FromTable(TableName).ForeignColumn("UserId")
                .ToTable("Users").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_RecentlyVisitedLinks_Users_UserId");
            Delete.Table(TableName);
        }
    }
}