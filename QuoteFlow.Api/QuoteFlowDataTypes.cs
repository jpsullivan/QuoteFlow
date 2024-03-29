﻿using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api
{
    /// <summary>
    /// Defines the known <see cref="IQuoteFlowDataType"/> data types"/>.
    /// </summary>
    public sealed class QuoteFlowDataTypes
    {
        /// <summary>
        /// Defines the core QuoteFlow data types
        /// </summary>
        public static readonly IQuoteFlowDataType Asset = new QuoteFlowDataType(typeof(Models.Asset));
        public static readonly IQuoteFlowDataType Catalog = new QuoteFlowDataType(typeof(Catalog));
        public static readonly IQuoteFlowDataType Manufacturer = new QuoteFlowDataType(typeof(Manufacturer));
//        public static readonly IQuoteFlowDataType PROJECT_CATEGORY = new QuoteFlowDataType(typeof(ProjectCategory));
//        public static readonly IQuoteFlowDataType VERSION = new QuoteFlowDataType(typeof(Version));
//        public static readonly IQuoteFlowDataType COMPONENT = new QuoteFlowDataType(typeof(ProjectComponent));
        public static readonly IQuoteFlowDataType User = new QuoteFlowDataType(typeof(User));
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

        public static readonly IQuoteFlowDataType All = new QuoteFlowDataType(typeof(object));

        private static readonly IDictionary<string, IQuoteFlowDataType> FieldMap = new Dictionary<string, IQuoteFlowDataType>
        {
            { AssetFieldConstants.Catalog, Catalog },
            { AssetFieldConstants.Manufacturer, Manufacturer },
            { AssetFieldConstants.Comment, Text },
            { AssetFieldConstants.Description, Text },
            { AssetFieldConstants.Summary, Text },
            { AssetFieldConstants.Created, Date },
            { AssetFieldConstants.Updated, Date }
        };

        public static IQuoteFlowDataType GetFieldType(string fieldId)
        {
            return FieldMap[fieldId];
        }
    }
}