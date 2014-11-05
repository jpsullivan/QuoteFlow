namespace QuoteFlow.Models.Assets.Index
{
    public class DocumentConstants
    {
        public const string LuceneSortFieldPrefix = "sort_";

        public const string CatalogId = "catalogId";

        public const string AssetId = "assetId";
        public const string AssetName = "name";
        public const string AssetDesc = "description";

        public const string AssetCurrentUser = "asset_current_user";
        public const string AssetCreator = "asset_creator";
        public const string AssetCreated = "created";
        public const string AssetUpdated = "updated";

        // A special field that is used for searching for EMPTY values
        public const string AssetNonEmptyFieldIds = "nonemptyfieldids";
        // A special field that is used for searching for constraining EMPTY value searches to the issues that are relevant
        public const string AssetVisibleFieldIds = "visiblefieldids";

        public const string SpecificUser = "specificuser";
        public const string SpecificGroup = "specificgroup";
    }
}