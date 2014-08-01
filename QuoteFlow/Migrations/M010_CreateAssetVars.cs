using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011821)]
    public class M010_CreateAssetVars : Migration
    {
        private const string TableName = "AssetVars";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("Description").AsString(300).Nullable()
                .WithColumn("ValueType").AsString(50).NotNullable()
                .WithColumn("OrganizationId").AsInt32().NotNullable()
                .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(1)
                .WithColumn("CreatedUtc").AsDateTime().NotNullable()
                .WithColumn("CreatedBy").AsInt32().NotNullable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}