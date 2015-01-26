using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search.Searchers.Util
{
    public abstract class AbstractDateSearchInputHelper : IDateSearcherInputHelper
    {
        private static readonly ConvertClauseResult ConvertClauseBadResult = new ConvertClauseResult(null, false);

        private readonly DateSearcherConfig _config;
        private readonly IJqlOperandResolver _operandResolver;

        public AbstractDateSearchInputHelper(DateSearcherConfig config, IJqlOperandResolver operandResolver)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (operandResolver == null)
            {
                throw new ArgumentNullException("operandResolver");
            }

            _config = config;
            _operandResolver = operandResolver;
        }

        protected internal class ParseDateResult
        {
            internal readonly string ParsedDate;
            internal readonly bool Fits;

            internal ParseDateResult(bool fits, string parsedDate)
            {
                Fits = fits;
                ParsedDate = parsedDate;
            }
        }
        
        public ConvertClauseResult ConvertClause(IClause clause, User user, bool allowTimeComponent)
        {
            if (clause == null)
            {
                return ConvertClauseBadResult;
            }

            // Lets get all the clauses for this field.
            IEnumerable<ITerminalClause> clauseList = ValidateClauseStructure(clause);
            if (clauseList == null)
            {
                return ConvertClauseBadResult;
            }

            bool fits = true;
            //we have some time related clauses, so lets do something with them.
            string afterValue = null, beforeValue = null, previousValue = null, nextValue = null;
            foreach (var terminalClause in clauseList)
            {
                // EMPTY is not supported for dates
                var operand = terminalClause.Operand;
                if (_operandResolver.IsEmptyOperand(operand))
                {
                    return ConvertClauseBadResult;
                }

                // We need to check if a function is being used (e.g. the "Now()" function)
                if (_operandResolver.IsFunctionOperand(operand))
                {
                    return ConvertClauseBadResult;
                }

                var list = _operandResolver.GetValues(user, operand, terminalClause);
                if ((list == null) || (list.Count() != 1))
                {
                    //Either something very bad happened, or we are getting more results than we can handle. In either
                    //case, just ignore.
                    return ConvertClauseBadResult;
                }
                QueryLiteral dateLiteral = list.First();

                // we could've got a single empty literal without using the empty operand; this is also not supported
                if (dateLiteral.IsEmpty)
                {
                    return ConvertClauseBadResult;
                }

                Operator @operator = terminalClause.Operator;

                if (@operator == Operator.LESS_THAN_EQUALS)
                {
                    string date = GetValidNavigatorPeriod(dateLiteral);
                    if (date != null)
                    {
                        if (nextValue == null)
                        {
                            nextValue = date;
                        }
                        else
                        {
                            //we already have a relative next, so don't do anything. We could actually be nice
                            //and use the younger date, but this would change the clause.
                            return ConvertClauseBadResult;
                        }
                    }
                    else
                    {
                        ParseDateResult result = GetValidNavigatorDate(dateLiteral, allowTimeComponent);
                        date = result.ParsedDate;
                        if (date != null)
                        {
                            if (beforeValue == null)
                            {
                                if (!result.Fits)
                                {
                                    fits = false;
                                }
                                beforeValue = date;
                            }
                            else
                            {
                                return ConvertClauseBadResult;
                            }
                        }
                        else
                        {
                            return ConvertClauseBadResult;
                        }

                    }
                }
                else if (@operator == Operator.GREATER_THAN_EQUALS)
                {
                    string date = GetValidNavigatorPeriod(dateLiteral);
                    if (date != null)
                    {
                        if (previousValue == null)
                        {
                            previousValue = date;
                        }
                        else
                        {
                            return ConvertClauseBadResult;
                        }
                    }
                    else
                    {
                        ParseDateResult result = GetValidNavigatorDate(dateLiteral, allowTimeComponent);
                        date = result.ParsedDate;
                        if (date != null)
                        {
                            if (afterValue == null)
                            {
                                if (!result.Fits)
                                {
                                    fits = false;
                                }
                                afterValue = date;
                            }
                            else
                            {
                                return ConvertClauseBadResult;
                            }

                        }
                        else
                        {
                            return ConvertClauseBadResult;
                        }
                    }
                }
                else
                {
                    return ConvertClauseBadResult;
                }
            }

            // we don't want to store the parameters which were not present in the clause
            var fields = new Dictionary<string, string>();
            if(beforeValue != null) fields.Add(_config.BeforeField, beforeValue);
            if(afterValue != null) fields.Add(_config.AfterField, afterValue);
            if (previousValue != null) fields.Add(_config.PreviousField, previousValue);
            if (nextValue != null) fields.Add(_config.NextField, nextValue);

            return new ConvertClauseResult(fields, fits);
        }

        internal abstract ParseDateResult GetValidNavigatorDate(QueryLiteral dateLiteral, bool allowTimeComponent);

        private string GetValidNavigatorPeriod(QueryLiteral dateLiteral)
        {
            if (dateLiteral.StringValue.HasValue())
            {
                try
                {
                    //DateUtils.getDurationWithNegative(dateLiteral.StringValue);
                    return dateLiteral.StringValue;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Checks the clause structure for validity, and returns the needed clauses from the tree if valid.
        /// </summary>
        /// <param name="clause"> the clause to check </param>
        /// <returns> a list of clauses for the field specified in the config, or null if the clause was invalid or there
        /// were no clauses in the tree </returns>
        internal virtual IEnumerable<ITerminalClause> ValidateClauseStructure(IClause clause)
        {
            var visitor = new SimpleNavigatorCollectorVisitor(_config.ClauseNames);
            clause.Accept(visitor);
            var clauses = visitor.Clauses;

            // If the clause was invalid (in any part), or there were such dates in clause, return null
            if (!visitor.Valid || clauses.Count == 0)
            {
                return null;
            }

            return clauses;
        }

    }
}