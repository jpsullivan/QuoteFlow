using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Api.Jql
{
    public class SearchRendererHolder
    {
        public bool IsValid { get; set; }
        public IClause Clause { get; set; }
        public IFieldValuesHolder FieldParams { get; set; }

        public SearchRendererHolder(bool valid, IClause clause, IFieldValuesHolder fieldParams)
        {
            IsValid = valid;
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