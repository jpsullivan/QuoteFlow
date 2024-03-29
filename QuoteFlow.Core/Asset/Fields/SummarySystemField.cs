﻿using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Parameters.Lucene.Sort;
using QuoteFlow.Api.Asset.Statistics;
using QuoteFlow.Core.Asset.Search.Handlers;

namespace QuoteFlow.Core.Asset.Fields
{
    public class SummarySystemField : AbstractTextSystemField, INavigableField<string>, ISummaryField
    {
        private const string SummaryNameKey = "asset.field.summary";
        private static readonly ILuceneFieldSorter<string> SORTER = new TextFieldSorter(DocumentConstants.AssetSortName);

        public SummarySystemField(SummarySearchHandlerFactory searchHandlerFactory)
            : base(AssetFieldConstants.Summary, SummaryNameKey, searchHandlerFactory)
        {
        }

        void IOrderableField.PopulateFromAsset(IDictionary<string, object> fieldValuesHolder, Api.Models.Asset asset)
        {
            fieldValuesHolder[Id] = asset.Name;
        }

        ILuceneFieldSorter<string> INavigableField<string>.Sorter => SORTER;

        IEnumerable<SortField> INavigableField.GetSortFields(bool sortOrder)
        {
            return new List<SortField>
            {
                new SortField(DocumentConstants.AssetSortName, new StringSortComparator(), sortOrder)
            };
        }
    }
}