using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Jql.Resolver;

namespace QuoteFlow.Api.Jql.Query
{
    /// <summary>
    /// Used to generate equality lucene queries. When this searches for EMPTY values it will search for the 
    /// absence of the field in the <see cref="DocumentConstants#ISSUE_NON_EMPTY_FIELD_IDS"/> field.
    /// </summary>
    public class EqualityQueryFactory<T> : AbstractEqualityQueryFactory<T>
    {
        public EqualityQueryFactory(IIndexInfoResolver<T> indexInfoResolver)
            : base(indexInfoResolver)
        {
        }

        public virtual QueryFactoryResult CreateQueryForEmptyOperand(string fieldName, Operator @operator)
        {
            if (@operator == Operator.IS || @operator == Operator.EQUALS)
            {
                return new QueryFactoryResult(GetIsEmptyQuery(fieldName));
            }
            if (@operator == Operator.IS_NOT || @operator == Operator.NOT_EQUALS)
            {
                return new QueryFactoryResult(GetIsNotEmptyQuery(fieldName));
            }
            //log.debug(string.Format("Cannot create a query for an empty operand using the operator '{0}'", @operator.DisplayString));
            return QueryFactoryResult.CreateFalseResult();
        }

        public override global::Lucene.Net.Search.Query GetIsEmptyQuery(string fieldName)
        {
            // We are returning a query that will include empties by specifying a MUST_NOT occurrance.
            // We should add the visibility query so that we exclude documents which don't have fieldName indexed.
            var result = new QueryFactoryResult(TermQueryFactory.NonEmptyQuery(fieldName), true);
            return QueryFactoryResult.WrapWithVisibilityQuery(fieldName, result).LuceneQuery;
        }

        public override global::Lucene.Net.Search.Query GetIsNotEmptyQuery(string fieldName)
        {
            return TermQueryFactory.NonEmptyQuery(fieldName);
        }
    }

}