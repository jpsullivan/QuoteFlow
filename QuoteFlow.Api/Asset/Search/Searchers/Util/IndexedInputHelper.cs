using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search.Searchers.Util
{
    /// <summary>
    /// The default implementation for the <see cref="IndexedInputHelper"/>.
    /// <p/>
    /// This class should be constructed as needed and not injected, as the <see cref="IndexInfoResolver"/> is only relevant to
    /// specific field(s).
    /// 
    /// @since v4.0
    /// </summary>
    public class IndexedInputHelper<T> : IIndexedInputHelper
    {
        private readonly IJqlOperandResolver operandResolver;
        private readonly IIndexInfoResolver<T> indexInfoResolver;
        private readonly IFieldFlagOperandRegistry fieldFlagOperandRegistry;

        public IndexedInputHelper(IIndexInfoResolver<T> indexInfoResolver, IJqlOperandResolver operandResolver, IFieldFlagOperandRegistry fieldFlagOperandRegistry)
        {
            this.operandResolver = operandResolver;
            this.indexInfoResolver = indexInfoResolver;
            this.fieldFlagOperandRegistry = fieldFlagOperandRegistry;
        }

        public virtual ISet<string> GetAllIndexValuesForMatchingClauses(User searcher, ClauseNames jqlClauseNames, IQuery query, ISearchContext searchContext)
        {
            return GetAllIndexValuesForMatchingClauses(searcher, jqlClauseNames, query);
        }

        public virtual ISet<string> GetAllIndexValuesForMatchingClauses(User searcher, ClauseNames jqlClauseNames, IQuery query)
        {
            var allValues = new HashSet<string>();
            var clauses = GetMatchingClauses(jqlClauseNames.JqlFieldNames, query);
            foreach (var clause in clauses)
            {
                foreach (var indexValue in GetAllIndexValuesAsStrings(searcher, clause.Operand, clause))
                {
                    allValues.Add(indexValue);
                }
            }
            return allValues;
        }

        public virtual ISet<string> GetAllNavigatorValuesForMatchingClauses(User searcher, ClauseNames jqlClauseNames, IQuery query, ISearchContext searchContext)
        {
            return GetAllNavigatorValuesForMatchingClauses(searcher, jqlClauseNames, query);
        }

        public virtual ISet<string> GetAllNavigatorValuesForMatchingClauses(User searcher, ClauseNames jqlClauseNames, IQuery query)
        {
            var allValues = new HashSet<string>();
            var clauses = GetMatchingClauses(jqlClauseNames.JqlFieldNames, query);
            foreach (var clause in clauses)
            {
                foreach (var navigatorValue in GetAllNavigatorValues(searcher, jqlClauseNames.PrimaryName, clause.Operand, clause))
                {
                    allValues.Add(navigatorValue);
                }
            }
            return allValues;
        }

        public virtual IClause GetClauseForNavigatorValues(string clauseName, IEnumerable<string> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var operandValues = new HashSet<IOperand>();
            bool containsList = false;
            foreach (string stringValue in values)
            {
                // Note: need a Long because we need to ensure the Clause searches on id and not name.
                IOperand operand = fieldFlagOperandRegistry.GetOperandForFlag(clauseName, stringValue);
                if (operand != null)
                {
                    operandValues.Add(operand);
                    containsList = operandResolver.IsListOperand(operand);
                }
                else
                {
                    operand = CreateOperand(stringValue);
                    operandValues.Add(operand);
                    containsList = operandResolver.IsListOperand(operand);
                }
            }

            if (operandValues.Count() == 1)
            {
                return new TerminalClause(clauseName, containsList ? Operator.IN : Operator.EQUALS, operandValues.First());
            }

            if (operandValues.Count() > 1)
            {
                return new TerminalClause(clauseName, Operator.IN, new MultiValueOperand(operandValues));
            }
            
            return null;
        }

        /// <summary>
        /// Resolve this string representation of a navigator value (known not to be a field flag) into an operand to be used
        /// in a clause. Override this to provide domain-specific resolution (e.g. resolve version ids to names). Default implementation
        /// delegates to <see cref="#createSingleValueOperandFromId"/>
        /// </summary>
        /// <param name="stringValue"> the navigator value as a string e.g. <code>123</code> </param>
        /// <returns> the operand which best represents this navigator value - either a string name or the id or whatever. </returns>
        private IOperand CreateOperand(string stringValue)
        {
            return CreateSingleValueOperandFromId(stringValue);
        }

        /// <summary>
        /// Resolve this string representation of a navigator value (known not to be a field flag) into an operand to be used
        /// in a clause (assuming that this is a single value operand). Override this to provide domain-specific resolution (e.g. resolve version ids to names).
        /// </summary>
        /// <param name="stringValue"> the navigator value as a string e.g. <code>123</code> </param>
        /// <returns> the operand which best represents this navigator value - either a string name or the id or whatever. </returns>
        protected virtual SingleValueOperand CreateSingleValueOperandFromId(string stringValue)
        {
            // if Long doesn't parse, then assume bad input but create clause anyway
            SingleValueOperand o;
            try
            {
                o = new SingleValueOperand(Convert.ToInt32(stringValue));
            }
            catch (Exception e)
            {
//                // we got some project id - we will fall back to using String as the operand
//                if (log.DebugEnabled)
//                {
//                    log.debug("Got a strange non-id input '" + stringValue + "' - continuing anyway so that clause is still constructed.");
//                }
                o = new SingleValueOperand(stringValue);
            }
            return o;
        }

        /// <summary>
        /// This method is used to implement <see cref="IIndexedInputHelper.GetAllNavigatorValuesForMatchingClauses"/>
        /// and must follow the contract as defined by <see cref="IIndexedInputHelper.GetAlNavigatorValues"/>.
        /// </summary>
        /// <param name="searcher"></param>
        /// <param name="fieldName"></param>
        /// <param name="operand"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        private IEnumerable<string> GetAllNavigatorValues(User searcher, string fieldName, IOperand operand, ITerminalClause clause)
        {
            // if we have a way to represent this operand as a navigator-specific flag, do it
            ISet<string> flags = fieldFlagOperandRegistry.GetFlagForOperand(fieldName, operand);
            if (flags != null)
            {
                return flags;
            }
            
            if (operand is MultiValueOperand)
            {
                var multiValueOperand = (MultiValueOperand) operand;
                ISet<string> values = new HashSet<string>();

                foreach (var navigatorValue in multiValueOperand.Values.SelectMany(sub => GetAllNavigatorValues(searcher, fieldName, sub, clause)))
                {
                    values.Add(navigatorValue);
                }
                return values;
            }

            return GetAllIndexValuesAsStrings(searcher, operand, clause);
        }

        private ISet<string> GetAllIndexValuesAsStrings(User searcher, IOperand operand, ITerminalClause clause)
        {
            ISet<string> allValues = new HashSet<string>();
            var values = operandResolver.GetValues(searcher, operand, clause);
            if (values != null)
            {
                foreach (QueryLiteral literal in values)
                {
                    IList<string> idsAsStrings;
                    if (literal.StringValue != null)
                    {
                        idsAsStrings = indexInfoResolver.GetIndexedValues(literal.StringValue);
                    }
                    else if (literal.IntValue != null)
                    {
                        idsAsStrings = indexInfoResolver.GetIndexedValues(literal.IntValue);
                    }
                    else
                    {
                        // empty literal or something unexpected; ignore
                        continue;
                    }

                    if (idsAsStrings != null)
                    {
                        foreach (var id in idsAsStrings)
                        {
                            allValues.Add(id);
                        }
                    }
                }
            }

            return allValues;
        }

        private IEnumerable<ITerminalClause> GetMatchingClauses(IEnumerable<string> jqlClauseNames, IQuery query)
        {
            var clauseVisitor = new NamedTerminalClauseCollectingVisitor(jqlClauseNames);
            if (query != null && query.WhereClause != null)
            {
                query.WhereClause.Accept(clauseVisitor);
                return clauseVisitor.NamedClauses;
            }
            return new List<ITerminalClause>();
        }
    }
}