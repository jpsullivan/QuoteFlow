using FluentMigrator;

namespace QuoteFlow.Migrations
{
    [Migration(201408011824)]
    public class M011_CreateAssetVarValues : Migration
    {
        private const string TableName = "AssetVarValues";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("AssetId").AsInt32().NotNullable()
                .WithColumn("VarValue").AsString(150).NotNullable()
                .WithColumn("AssetVarId").AsInt32().NotNullable()
                .WithColumn("OrganizationId").AsInt32().NotNullable();

            Create.ForeignKey("FK_AssetVarValues_AssetVars_Id")
                .FromTable(TableName).ForeignColumn("AssetVarId")
                .ToTable("AssetVars").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_AssetVarValues_AssetVars_Id");
            Delete.Table(TableName);
        }
    }
}