using System;
using System.Collections.Generic;
using QuoteFlow.Models;
using QuoteFlow.Models.Assets;
using QuoteFlow.Models.Assets.Fields;

namespace QuoteFlow.Infrastructure
{
    /// <summary>
    /// Defines the known <see cref="IQuoteFlowDataType"/> data types"/>.
    /// </summary>
    public sealed class QuoteFlowDataTypes
    {
        /// <summary>
        /// Defines the core QuoteFlow data types
        /// </summary>
        public static readonly IQuoteFlowDataType Asset = new QuoteFlowDataType(typeof(Asset));
        public static readonly IQuoteFlowDataType Catalog = new QuoteFlowDataType(typeof(Catalog));
//        public static readonly IQuoteFlowDataType PROJECT_CATEGORY = new QuoteFlowDataType(typeof(ProjectCategory));
//        public static readonly IQuoteFlowDataType VERSION = new QuoteFlowDataType(typeof(Version));
//        public static readonly IQuoteFlowDataType COMPONENT = new QuoteFlowDataType(typeof(ProjectComponent));
//        public static readonly IQuoteFlowDataType USER = new QuoteFlowDataType(typeof(User));
//        public static readonly IQuoteFlowDataType GROUP = new QuoteFlowDataType(typeof(Group));
//        public static readonly IQuoteFlowDataType PROJECT_ROLE = new QuoteFlowDataType(typeof(ProjectRole));
//        public static readonly IQuoteFlowDataType PRIORITY = new QuoteFlowDataType(typeof(Priority));
//        public static readonly IQuoteFlowDataType RESOLUTION = new QuoteFlowDataType(typeof(Resolution));
//        public static readonly IQuoteFlowDataType ISSUE_TYPE = new QuoteFlowDataType(typeof(IssueType));
//        public static readonly IQuoteFlowDataType STATUS = new QuoteFlowDataType(typeof(Status));
//        public static readonly IQuoteFlowDataType STATUS_CATEGORY = new QuoteFlowDataType(typeof(StatusCategory));
//        public static readonly IQuoteFlowDataType CASCADING_OPTION = new QuoteFlowDataType(typeof(CascadingOption));
//        public static readonly IQuoteFlowDataType OPTION = new QuoteFlowDataType(typeof(Option));
//        public static readonly IQuoteFlowDataType SAVED_FILTER = new QuoteFlowDataType(typeof(SearchRequest));
//        public static readonly IQuoteFlowDataType ISSUE_SECURITY_LEVEL = new QuoteFlowDataType(typeof(IssueSecurityLevel));
//        public static readonly IQuoteFlowDataType LABEL = new QuoteFlowDataType(typeof(Label));
//        public static readonly IQuoteFlowDataType ATTACHMENT = new QuoteFlowDataType(typeof(Attachment));

        public static readonly IQuoteFlowDataType Date = new QuoteFlowDataType(typeof(DateTime));
        public static readonly IQuoteFlowDataType Text = new QuoteFlowDataType(typeof(String));
        public static readonly IQuoteFlowDataType Number = new QuoteFlowDataType(typeof(Int32));
//        public static readonly IQuoteFlowDataType DURATION = new QuoteFlowDataType(typeof(Duration));

        public static readonly IQuoteFlowDataType ALL = new QuoteFlowDataType(typeof(object));

        private static readonly IDictionary<string, IQuoteFlowDataType> FieldMap = new Dictionary<string, IQuoteFlowDataType>()
        {
            { AssetFieldConstants.Catalog, IQuoteFlowDataType.Catalog },
            { AssetFieldConstants.Comment, IQuoteFlowDataType.TEXT },
            { AssetFieldConstants.Description, IQuoteFlowDataType.TEXT },
            { AssetFieldConstants.Summary, IQuoteFlowDataType.TEXT },
            { AssetFieldConstants.Created, IQuoteFlowDataType.DATE },
            { AssetFieldConstants.Updated, IQuoteFlowDataType.DATE }
        };

        public static string GetType(IField field)
        {
            if (field is CommentField)
            {
                return field.GetType().CanonicalName;
            }

            IQuoteFlowDataType dataType = GetFieldType(field.Id);
            if (dataType == null)
            {
                return field.GetType().CanonicalName;
            }

            ICollection<string> stringCollection = dataType.AsStrings();
            if (stringCollection.Count == 1)
            {
                return stringCollection.GetEnumerator().MoveNext();
            }
            return stringCollection.ToString();
        }

        // This is primarily for generating REST documentation and other such things where you
        // can't get a Field easily. In real production code you should probably be using the other version
        public static string GetType(string fieldId)
        {
            IQuoteFlowDataType dataType = GetFieldType(fieldId);
            if (dataType == null)
            {
                return fieldId;
            }
            ICollection<string> stringCollection = dataType.AsStrings();
            if (stringCollection.Count == 1)
            {
                return stringCollection.GetEnumerator().MoveNext();
            }
            return stringCollection.ToString();
        }

        public static IQuoteFlowDataType GetFieldType(string fieldId)
        {
            return FieldMap[fieldId];
        }

    }
}