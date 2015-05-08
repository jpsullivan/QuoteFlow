using System.Collections.Generic;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Jql.Query;

namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class QuoteBuilderViewModel
    {
        public QuoteBuilderViewModel(Quote quote, AssetTable assetTable, IEnumerable<string> visibleFieldNames, IEnumerable<string> visibleFunctionNames, IEnumerable<string> jqlReservedWords, IServiceOutcome<QuerySearchResults> criteriaJqlOutcome)
        {
            Quote = quote;
            AssetTable = assetTable;
            VisibleFieldNames = visibleFieldNames;
            VisibleFunctionNames = visibleFunctionNames;
            JqlReservedWords = jqlReservedWords;
            CriteriaJqlOutcome = criteriaJqlOutcome;
        }

        public Quote Quote { get; set; }
        public AssetTable AssetTable { get; set; }

        public IEnumerable<string> VisibleFieldNames { get; set; }
        public IEnumerable<string> VisibleFunctionNames { get; set; } 
        public IEnumerable<string> JqlReservedWords { get; set; }

        public IServiceOutcome<QuerySearchResults> CriteriaJqlOutcome { get; set; } 
    }
}
