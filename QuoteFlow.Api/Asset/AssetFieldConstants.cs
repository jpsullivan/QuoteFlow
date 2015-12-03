using System.Collections.Generic;

namespace QuoteFlow.Api.Asset
{
    public class AssetFieldConstants
    {
        public const string Catalog = "catalog";
        public const string FORM_TOKEN = "formToken";
        public const string Cost = "cost";
        public const string Creator = "creator";
        public const string Comment = "comment";
        public const string Description = "description";
        public const string Manufacturer = "manufacturer";
        public const string THUMBNAIL = "thumbnail";
        public const string LAST_VIEWED = "lastViewed";
        public const string Summary = "summary";
        public const string Sku = "sku";
        public const string Created = "created";
        public const string Updated = "updated";

        static AssetFieldConstants()
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            result[Description] = "Description";
            result[Created] = "Created";
            result[Summary] = "Summary";
            result[Sku] = "Sku";
            result[Cost] = "Cost";
            FieldIdsToLabels = new Dictionary<string, string>(result);
        }

        public static IDictionary<string, string> FieldIdsToLabels { get; private set; }

        public static bool IsRequiredField(string field)
        {
            return "summary".Equals(field) || "manufacturer".Equals(field);
        }
    }
}