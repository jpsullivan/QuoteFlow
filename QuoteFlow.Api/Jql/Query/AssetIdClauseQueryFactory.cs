using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Api.Jql.Query
{
    /// <summary>
    /// A <see cref="IClauseQueryFactory"/> for the "Asset ID" JQL clause.
    /// </summary>
    public class AssetIdClauseQueryFactory : IClauseQueryFactory
    {
        private readonly IJqlOperandResolver _operandResolver;
		private readonly IJqlAssetSupport _assetSupport;
        private readonly ICatalogService _catalogService;

        public AssetIdClauseQueryFactory(IJqlOperandResolver operandResolver, IJqlAssetSupport assetSupport, ICatalogService catalogService)
		{
            if (operandResolver == null) throw new ArgumentNullException(nameof(operandResolver));
            if (assetSupport == null) throw new ArgumentNullException(nameof(assetSupport));
            if (catalogService == null) throw new ArgumentNullException(nameof(catalogService));

            _assetSupport = assetSupport;
			_operandResolver = operandResolver;
            _catalogService = catalogService;
		}

        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            if (queryCreationContext == null)
            {
                throw new ArgumentNullException(nameof(queryCreationContext));
            }

            var operand = terminalClause.Operand;
            Operator @operator = terminalClause.Operator;

            if (OperatorClasses.EmptyOnlyOperators.Contains(@operator) && !operand.Equals(EmptyOperand.Empty))
            {
                return QueryFactoryResult.CreateFalseResult();
            }

            var literals = _operandResolver.GetValues(queryCreationContext, operand, terminalClause);

            if (literals == null)
            {
                //log.debug(string.Format("Unable to find operand values from operand '{0}' for clause '{1}'.", operand.DisplayString, terminalClause.Name));
                return QueryFactoryResult.CreateFalseResult();
            }
            if (isEqualityOperator(@operator))
            {
                return HandleEquals(literals);
            }
            if (isNegationOperator(@operator))
            {
                return HandleNotEquals(literals);
            }
            if (OperatorClasses.RelationalOnlyOperators.Contains(@operator))
            {
                if (_operandResolver.IsListOperand(operand))
                {
                    //log.debug(string.Format("Tried to use list operand '{0}' with relational operator '{1}' in clause '{2}'.", operand.DisplayString, @operator.DisplayString, terminalClause.Name));
                    return QueryFactoryResult.CreateFalseResult();
                }
                QueryLiteral literal = _operandResolver.GetSingleValue(queryCreationContext.User, operand, terminalClause);
                return HandleRelational(queryCreationContext.User, queryCreationContext.SecurityOverriden, @operator, literal, terminalClause);
            }
            //log.debug(string.Format("The '{0}' clause does not support the {1} operator.", terminalClause.Name, @operator));
            return QueryFactoryResult.CreateFalseResult();
        }

        private QueryFactoryResult HandleRelational(User user, bool overrideSecurity, Operator @operator, QueryLiteral literal, ITerminalClause clause)
        {
            return HandleRelational(user, overrideSecurity, literal, clause, CreateRangeQueryGenerator(@operator));
        }

        private QueryFactoryResult HandleRelational(User user, bool overrideSecurity, QueryLiteral literal, ITerminalClause clause, IRangeQueryGenerator rangeQueryGenerator)
        {
            if (literal.IsEmpty)
            {
                //log.debug(string.Format("Encountered EMPTY literal from operand '{0}' for operator '{1}' on clause '{2}'. Ignoring.", clause.Operand.DisplayString, clause.Operator.DisplayString, clause.Name));
            }

            IAsset asset;
            if (literal.IntValue != null)
            {
                asset = overrideSecurity ? _assetSupport.GetAsset((int) literal.IntValue) : _assetSupport.GetAsset((int) literal.IntValue, user);
            }
            else
            {
                //log.debug(string.Format("Encountered weird literal from operand '{0}' for operator '{1}' on clause '{2}'. Ignoring.", clause.Operand.DisplayString, clause.Operator.DisplayString, clause.Name));
                asset = null;
            }

            if (asset != null)
            {
                int currentCount = asset.Id;
                if (currentCount < 0)
                {
                    return QueryFactoryResult.CreateFalseResult();
                }
                var subQuery = new BooleanQuery
                {
                    {rangeQueryGenerator.Get(currentCount), Occur.MUST},
                    {CreateCatalogQuery(_catalogService.GetCatalog(asset.CatalogId)), Occur.MUST}
                };

                return new QueryFactoryResult(subQuery);
            }
            
            return QueryFactoryResult.CreateFalseResult();
        }

        private QueryFactoryResult HandleNotEquals(IEnumerable<QueryLiteral> rawValues)
        {
            return new QueryFactoryResult(CreatePositiveEqualsQuery(rawValues), true);
        }

        private QueryFactoryResult HandleEquals(IEnumerable<QueryLiteral> rawValues)
        {
            return new QueryFactoryResult(CreatePositiveEqualsQuery(rawValues));
        }

        private global::Lucene.Net.Search.Query CreatePositiveEqualsQuery(IEnumerable<QueryLiteral> rawValues)
        {
            if (rawValues.Count() == 1)
            {
                return CreateQuery(rawValues.ElementAt(0));
            }
            
            var query = new BooleanQuery();
            foreach (QueryLiteral rawValue in rawValues)
            {
                if (!rawValue.IsEmpty)
                {
                    query.Add(CreateQuery(rawValue), Occur.SHOULD);
                }
            }
            return query;
        }

        private static global::Lucene.Net.Search.Query CreateQuery(QueryLiteral rawValue)
        {
            if (!rawValue.IsEmpty)
            {
                return CreateQueryForNotEmptyValue(rawValue);
            }
            return new BooleanQuery();
        }

        private static global::Lucene.Net.Search.Query CreateQueryForNotEmptyValue(QueryLiteral rawValue)
        {
            return CreateQueryForId(rawValue.AsString());
        }

        private static global::Lucene.Net.Search.Query CreateQueryForId(string id)
        {
            string fieldName = SystemSearchConstants.ForAssetId().IndexField;
            return new TermQuery(new Term(fieldName, id));
        }

        private static global::Lucene.Net.Search.Query CreateCatalogQuery(Catalog project)
        {
            return new TermQuery(new Term(SystemSearchConstants.ForCatalog().IndexField, project.Id.ToString()));
        }

        private static global::Lucene.Net.Search.Query CreateRangeQuery(int min, int max, bool minInclusive, bool maxInclusive)
        {
            return new TermRangeQuery(SystemSearchConstants.ForAssetId().IndexField, min.ToString(), max.ToString(), minInclusive, maxInclusive);
        }

        private bool isNegationOperator(Operator @operator)
        {
            return (@operator == Operator.NOT_EQUALS) || (@operator == Operator.NOT_IN) || (@operator == Operator.IS_NOT);
        }

        private bool isEqualityOperator(Operator @operator)
        {
            return (@operator == Operator.EQUALS) || (@operator == Operator.IN) || (@operator == Operator.IS);
        }

        private static QueryFactoryResult CreateResult(IList<BooleanClause> clauses)
        {
            if (clauses.Count == 0)
            {
                return QueryFactoryResult.CreateFalseResult();
            }
            if (clauses.Count == 1)
            {
                return new QueryFactoryResult(clauses[0].Query);
            }
            
            var query = new BooleanQuery();
            foreach (BooleanClause clause in clauses)
            {
                query.Add(clause);
            }
            return new QueryFactoryResult(query);
        }

        private static IRangeQueryGenerator CreateRangeQueryGenerator(Operator @operator)
        {
            switch (@operator)
            {
                case Operator.LESS_THAN:
                    return new LessThanRangeQuery();
                case Operator.LESS_THAN_EQUALS:
                    return new LessThanEqualsRangeQuery();
                case Operator.GREATER_THAN:
                    return new GreaterThanRangeQuery();
                case Operator.GREATER_THAN_EQUALS:
                    return new GreaterThanEqualsRangeQuery();
                default:
                    throw new ArgumentException("Unsupported Operator:" + @operator);
            }
        }

        private class LessThanRangeQuery : IRangeQueryGenerator
        {
            public global::Lucene.Net.Search.Query Get(int limit)
            {
                return CreateRangeQuery(-1, limit, true, false);
            }
        }

        private class LessThanEqualsRangeQuery : IRangeQueryGenerator
        {
            public global::Lucene.Net.Search.Query Get(int limit)
            {
                return CreateRangeQuery(-1, limit, true, true);
            }
        }

        private class GreaterThanRangeQuery : IRangeQueryGenerator
        {
            public global::Lucene.Net.Search.Query Get(int limit)
            {
                return CreateRangeQuery(limit, -1, false, true);
            }
        }

        private class GreaterThanEqualsRangeQuery : IRangeQueryGenerator
        {
            public global::Lucene.Net.Search.Query Get(int limit)
            {
                return CreateRangeQuery(limit, -1, true, true);
            }
        }
    }

    interface IRangeQueryGenerator
    {
        global::Lucene.Net.Search.Query Get(int limit);
    }
}