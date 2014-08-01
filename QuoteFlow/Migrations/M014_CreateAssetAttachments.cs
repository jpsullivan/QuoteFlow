using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011844)]
    public class M014_CreateAssetAttachments : Migration
    {
        private const string TableName = "AssetAttachments";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("AssetId").AsInt32().NotNullable()
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("Path").AsString(255).NotNullable()
                .WithColumn("CreationDate").AsDateTime().NotNullable()
                .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(1);


            Create.ForeignKey("FK_AssetAttachments_Assets_Id")
                .FromTable(TableName).ForeignColumn("AssetId")
                .ToTable("Assets").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_AssetAttachments_Assets_Id");
            Delete.Table(TableName);
        }
    }
}