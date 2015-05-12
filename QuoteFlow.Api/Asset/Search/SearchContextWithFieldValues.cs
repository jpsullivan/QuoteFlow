using QuoteFlow.Api.Asset.Transport;

namespace QuoteFlow.Api.Asset.Search
{
    public class SearchContextWithFieldValues
    {
        public readonly ISearchContext SearchContext;
        public readonly IFieldValuesHolder FieldValuesHolder;

        public SearchContextWithFieldValues(ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder)
		{
			SearchContext = searchContext;
			FieldValuesHolder = fieldValuesHolder;
		}
    }
}