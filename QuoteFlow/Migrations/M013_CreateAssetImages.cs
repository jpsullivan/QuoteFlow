using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011841)]
    public class M013_CreateAssetImages : Migration
    {
        private const string TableName = "AssetImages";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("AssetId").AsInt32().NotNullable()
                .WithColumn("Path").AsString(255).NotNullable()
                .WithColumn("ThumbnailPath").AsString(255).NotNullable()
                .WithColumn("Caption").AsString(200).Nullable()
                .WithColumn("Type").AsString(20).NotNullable();


            Create.ForeignKey("FK_AssetImages_Assets_Id")
                .FromTable(TableName).ForeignColumn("AssetId")
                .ToTable("Assets").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_AssetImages_Assets_Id");
            Delete.Table(TableName);
        }
    }
}