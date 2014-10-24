﻿namespace QuoteFlow.Models.Assets.Index
{
    public class DocumentConstants
    {
        public const string LuceneSortFieldPrefix = "sort_";

        public const string CatalogId = "catalogId";

        public const string AssetId = "assetId";
        public const string AssetName = "name";

        // A special field that is used for searching for EMPTY values
        public const string AssetNonEmptyFieldIds = "nonemptyfieldids";
        // A special field that is used for searching for constraining EMPTY value searches to the issues that are relevant
        public const string AssetVisibleFieldIds = "visiblefieldids";
    }
}