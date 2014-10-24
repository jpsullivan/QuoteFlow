using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Transport;

namespace QuoteFlow.Models.Search.Jql
{
    public class SearchRendererHolder
    {
        public bool ValidRenamed { get; set; }
        public IClause Clause { get; set; }
        public IFieldValuesHolder FieldParams { get; set; }

        public SearchRendererHolder(bool valid, IClause clause, IFieldValuesHolder fieldParams)
        {
            ValidRenamed = valid;
            Clause = clause;
            FieldParams = fieldParams;
        }

        public static SearchRendererHolder Invalid(IClause clause)
        {
            return new SearchRendererHolder(false, clause, null);
        }

        public static SearchRendererHolder Valid(IClause clause, IFieldValuesHolder fieldParams)
        {
            return new SearchRendererHolder(true, clause, fieldParams);
        }
    }
}