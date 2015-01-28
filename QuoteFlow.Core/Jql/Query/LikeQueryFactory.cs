using System;
using System.Collections.Generic;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Ninject;
using QuoteFlow.Api.Asset.Search.Searchers.Util;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Lucene.Parsing;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// A factory for creating a Query for the <see cref="Operator.LIKE"/> equals operator.
    /// </summary>
    public class LikeQueryFactory : IOperatorSpecificQueryFactory
    {
        private readonly bool _usesMainIndex;

        public LikeQueryFactory()
        {
            _usesMainIndex = true;
        }

        public LikeQueryFactory(bool usesMainIndex)
        {
            _usesMainIndex = usesMainIndex;
        }


        public QueryFactoryResult CreateQueryForSingleValue(string fieldName, Operator oprator, List<QueryLiteral> rawValues)
        {
            if (oprator != Operator.LIKE && oprator != Operator.NOT_LIKE)
            {
//                if (log.DebugEnabled)
//                {
//                    log.debug(string.Format("Operator '{0}' is not a LIKE operator.", oprator.DisplayString));
//                }
                return QueryFactoryResult.CreateFalseResult();
            }

            if (rawValues == null)
            {
                return QueryFactoryResult.CreateFalseResult();
            }

            return CreateResult(fieldName, rawValues, oprator, _usesMainIndex);
        }

        public virtual QueryFactoryResult CreateResult(string fieldName, IEnumerable<QueryLiteral> rawValues, Operator @operator, bool handleEmpty)
        {
            IList<global::Lucene.Net.Search.Query> queries = GetQueries(fieldName, rawValues);
            if (queries == null || queries.Count == 0)
            {
                return QueryFactoryResult.CreateFalseResult();
            }

            var fullQuery = new BooleanQuery();
            bool hasEmpty = false;

            if (queries.Count == 1)
            {
                if (queries[0] == null && handleEmpty)
                {
                    return CreateQueryForEmptyOperand(fieldName, @operator);
                }
                
                fullQuery.Add(queries[0], @operator == Operator.NOT_LIKE ? Occur.MUST_NOT : Occur.MUST);
            }
            else
            {
                var subQuery = new BooleanQuery();
                foreach (var query in queries)
                {
                    if (query == null)
                    {
                        hasEmpty = true;
                    }
                    else
                    {
                        subQuery.Add(query, @operator == Operator.NOT_LIKE ? Occur.MUST_NOT : Occur.SHOULD);
                    }
                }
                if (handleEmpty && hasEmpty)
                {
                    subQuery.Add(CreateQueryForEmptyOperand(fieldName, @operator).LuceneQuery, @operator == Operator.NOT_LIKE ? Occur.MUST : Occur.SHOULD);
                }
                fullQuery.Add(subQuery, Occur.MUST);
            }

            if (handleEmpty && !hasEmpty)
            {
                // For both LIKE and NOT_LIKE we need to add the exclude empty clause because their query could be a negative query
                // generated from the Lucene search syntax. We also need a visibility query in case this field is not searchable.
                fullQuery.Add(TermQueryFactory.NonEmptyQuery(fieldName), Occur.MUST);
                fullQuery.Add(TermQueryFactory.VisibilityQuery(fieldName), Occur.MUST);
            }

            return new QueryFactoryResult(fullQuery);
        }

        private IList<global::Lucene.Net.Search.Query> GetQueries(string fieldName, IEnumerable<QueryLiteral> rawValues)
        {
            QueryParser parser = GetQueryParser(fieldName);
            parser.DefaultOperator = QueryParser.Operator.AND;
            var queries = new List<global::Lucene.Net.Search.Query>();
            foreach (QueryLiteral rawValue in rawValues)
            {
                if (rawValue.IsEmpty)
                {
                    queries.Add(null);
                }
                else if (rawValue.AsString().HasValue())
                {
                    global::Lucene.Net.Search.Query query;
                    try
                    {
                        string value = getEscapedValueFromRawValues(rawValue);
                        query = parser.Parse(value);
                    }
                    catch (ParseException e)
                    {
//                        if (log.DebugEnabled)
//                        {
//                            log.debug(string.Format("Unable to parse the text '{0}' for field '{1}'.", rawValue.asString(), fieldName));
//                        }
                        return null;
                    }
                    catch (Exception e)
                    {
//                        // JRA-27018  FuzzyQuery throws IllegalArgumentException instead of ParseException
//                        if (log.DebugEnabled)
//                        {
//                            log.debug(string.Format("Unable to parse the text '{0}' for field '{1}'.", rawValue.asString(), fieldName));
//                        }
                        return null;
                    }
                    queries.Add(query);
                }
            }
            return queries;
        }

        protected virtual QueryParser GetQueryParser(string fieldName)
        {
            return Container.Kernel.TryGet<ILuceneQueryParserFactory>().CreateParserFor(fieldName);
        }

        public QueryFactoryResult CreateQueryForMultipleValues(string fieldName, Operator @operator, List<QueryLiteral> rawValues)
        {
//            if (log.DebugEnabled)
//            {
//                log.debug("LIKE clauses do not support multi value operands.");
//            }
            return QueryFactoryResult.CreateFalseResult();
        }

        public QueryFactoryResult CreateQueryForEmptyOperand(string fieldName, Operator oprator)
        {
            if (oprator == Operator.IS || oprator == Operator.LIKE)
            {
                // We are returning a query that will include empties by specifying a MUST_NOT occurrance.
                // We should add the visibility query so that we exclude documents which don't have fieldName indexed.
                var result = new QueryFactoryResult(TermQueryFactory.NonEmptyQuery(fieldName), true);
                return QueryFactoryResult.WrapWithVisibilityQuery(fieldName, result);
            }
            if (oprator == Operator.IS_NOT || oprator == Operator.NOT_LIKE)
            {
                return new QueryFactoryResult(TermQueryFactory.NonEmptyQuery(fieldName));
            }
//            if (log.DebugEnabled)
//            {
//                log.debug(string.Format("Create query for empty operand was called with operator '{0}', this only handles '=', '!=', 'is' and 'not is'.", @operator.DisplayString));
//            }
            return QueryFactoryResult.CreateFalseResult();
        }

        private string getEscapedValueFromRawValues(QueryLiteral rawValue)
        {
            if (rawValue.IsEmpty)
            {
                return null;
            }
            string value = rawValue.AsString();

            // NOTE: we need this so that we do not allow users to search a different field by specifying 'field:val'
            // we only want them to search the field they have specified via the JQL.
            return TextTermEscaper.Escape(value.ToCharArray());
        }

        public bool HandlesOperator(Operator oprator)
        {
            return OperatorClasses.TextOperators.Contains(oprator);
        }
    }
}