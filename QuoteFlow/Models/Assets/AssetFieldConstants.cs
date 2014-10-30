using System.Collections.Generic;

namespace QuoteFlow.Models.Assets
{
    public class AssetFieldConstants
    {
        private static readonly IDictionary<string, string> fieldIdsToLabels;
        public const string Catalog = "catalog";
        public const string FORM_TOKEN = "formToken";
        public const string Creator = "creator";
        public const string Comment = "comment";
        public const string Description = "description";
        public const string AssetType = "assettype";
        public const string THUMBNAIL = "thumbnail";
        public const string LAST_VIEWED = "lastViewed";
        public const string Summary = "summary";
        public const string Created = "created";
        public const string Updated = "updated";

        static AssetFieldConstants()
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            result[Description] = "Description";
            result[Created] = "Created";
            result[Summary] = "Summary";
            fieldIdsToLabels = new Dictionary<string, string>(result);
        }

        public static IDictionary<string, string> FieldIdsToLabels
        {
            get { return fieldIdsToLabels; }
        }

        public static bool IsRequiredField(string field)
        {
            return ("summary".Equals(field) || "assettype".Equals(field));
        }
    }

}