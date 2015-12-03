using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search.Searchers.Util
{
    public class CostSearcherInputHelper : ICostSearcherInputHelper
    {
        private readonly SimpleFieldSearchConstants _constants;
        private readonly CostSearcherConfig _config;
        private readonly IJqlOperandResolver _operandResolver;

        public CostSearcherInputHelper(SimpleFieldSearchConstants constants, IJqlOperandResolver operandResolver)
        {
            if (constants == null) throw new ArgumentNullException(nameof(constants));
            if (operandResolver == null) throw new ArgumentNullException(nameof(operandResolver));

            _constants = constants;
            _config = new CostSearcherConfig(constants.SearcherId);
            _operandResolver = operandResolver;
        }

        public IDictionary<string, string> ConvertClause(IClause clause, User user)
        {
            if (clause == null)
            {
                return null;
            }

            // lets get all the clauses for this field
            var clauseList = ValidateClauseStructure(clause);
            if (clauseList == null)
            {
                return null;
            }

            // we have some time related clauses, so let's do something with 'em
            string minValue = null;
            string maxValue = null;
            foreach (var terminalClause in clauseList)
            {
                // EMPTY is not supported for cost searcher
                var operand = terminalClause.Operand;
                if (_operandResolver.IsEmptyOperand(operand))
                {
                    return null;
                }

                var list = _operandResolver.GetValues(user, operand, terminalClause).ToList();
                if (list.Count != 1)
                {
                    // either something very bad happened, or we are getting more results than
                    // we can handle. In either case, just ignore.
                    return null;
                }

                QueryLiteral ratioLiteral = list.First();

                // we could've got a single empty literal with using the empty operand;
                // this is also not supported
                if (ratioLiteral.IsEmpty)
                {
                    return null;
                }

                var oprator = terminalClause.Operator;
                if (oprator == Operator.LESS_THAN_EQUALS)
                {
                    var ratio = GetRatioFromLiteral(ratioLiteral);
                    if (ratio != null)
                    {
                        if (maxValue == null)
                        {
                            maxValue = ratio;
                        }
                        else
                        {
                            // we already have a max ratio before, so don't do anything.
                            // We could actually be nice and use the smaller maximum,
                            // but this would change the clause.
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (oprator == Operator.GREATER_THAN_EQUALS)
                {
                    var ratio = GetRatioFromLiteral(ratioLiteral);
                    if (ratio != null)
                    {
                        if (minValue == null)
                        {
                            minValue = ratio;
                        }
                        else
                        {
                            // we already have a max ratio before, so don't do anything.
                            // We could actually be nice and use the smaller maximum,
                            // but this would change the clause.
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            // we don't want to store the params which were not present in the clause
            var result = new Dictionary<string, string>();
            if (maxValue != null)
            {
                result.Add(_config.Max, maxValue);
            }
            if (minValue != null)
            {
                result.Add(_config.Min, minValue);
            }

            return result;
        }

        private IList<ITerminalClause> ValidateClauseStructure(IClause clause)
        {
            var visitor = new SimpleNavigatorCollectorVisitor(_constants.JqlClauseNames);
            clause.Accept(visitor);
            var clauses = visitor.Clauses;

            // if the clause was invalid (in any part), or there were no clauses, return null
            if (!visitor.Valid || !clauses.Any())
            {
                return null;
            }

            return clauses;
        }

        private static string GetRatioFromLiteral(QueryLiteral ratioLiteral)
        {
            if (ratioLiteral.IntValue != null)
            {
                return ratioLiteral.IntValue.ToString();
            }

            if (!string.IsNullOrEmpty(ratioLiteral.StringValue))
            {
                return ratioLiteral.StringValue;
            }

            return null;
        }
    }
}