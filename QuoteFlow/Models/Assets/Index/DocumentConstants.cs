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

        // fields that are just indexed for sorting purposes
        public const string AssetSortName = LuceneSortFieldPrefix + "name";
        public const string AssetSortDesc = LuceneSortFieldPrefix + "description";
        public const string AssetSortCreated = LuceneSortFieldPrefix + "created";
        public const string AssetSortUpdated = LuceneSortFieldPrefix + "updated";

        // extra constants
        public const string ChangeDuration ="ch_duration";
        public const string ChangeDate = "ch_date";
        public const string NextChangeDate="ch_nextchangedate";
        public const string ChangeGroupId="ch_id";
        public const string ChangeActioner = "ch_who";
        public const string ChangeFrom = "ch_from" ;
        public const string ChangeTo = "ch_to" ;
        public const string OldValue = "ch_oldvalue";
        public const string NewValue = "ch_newvalue";
        public const string ChangeHistoryProtocol="ch-";

        // A special field that is used for searching for EMPTY values
        public const string AssetNonEmptyFieldIds = "nonemptyfieldids";
        // A special field that is used for searching for constraining EMPTY value searches to the issues that are relevant
        public const string AssetVisibleFieldIds = "visiblefieldids";

        public const string SpecificUser = "specificuser";
        public const string SpecificGroup = "specificgroup";
    }
}