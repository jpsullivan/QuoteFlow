using FluentMigrator;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Migrations
{
    [Migration(201408011837)]
    public class M012_CreateAssetComments : Migration
    {
        private const string TableName = "AssetComments";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("Comment").AsMaxString().NotNullable()
                .WithColumn("CreatorId").AsInt32().NotNullable()
                .WithColumn("AssetId").AsInt32().NotNullable()
                .WithColumn("CreatedUtc").AsDateTime().NotNullable();

            Create.ForeignKey("FK_AssetComments_Users_Id")
                .FromTable(TableName).ForeignColumn("CreatorId")
                .ToTable("Users").PrimaryColumn("Id");

            Create.ForeignKey("FK_AssetComments_Assets_Id")
                .FromTable(TableName).ForeignColumn("AssetId")
                .ToTable("Assets").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_AssetComments_Assets_Id");
            Delete.ForeignKey("FK_AssetComments_Users_Id");
            Delete.Table(TableName);
        }
    }
}