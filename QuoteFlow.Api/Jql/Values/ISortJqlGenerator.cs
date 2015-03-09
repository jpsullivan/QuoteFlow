using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Jql.Query;

namespace QuoteFlow.Api.Jql.Values
{
    public interface ISortJqlGenerator
    {
        IDictionary<string, string> GenerateColumnSortJql(IQuery query, IEnumerable<INavigableField> fields);
    }
}
