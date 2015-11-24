using System;
using System.Web.Mvc;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// A transformer that handles the 2012 Issue Search interface.
    /// </summary>
    public class TextQuerySearchInputTransformer : AbstractSearchInputTransformer
    {
        private readonly ClauseNames clauseNames;

        public TextQuerySearchInputTransformer(string id, IClauseInformation information, IJqlOperandResolver operandResolver)
            : base(operandResolver, id, id)
        {
            clauseNames = information.JqlClauseNames;
        }

        public override bool DoRelevantClausesFitFilterForm(User user, IQuery query, ISearchContext searchContext)
        {
            if (query != null && query.WhereClause != null)
            {
                var whereClause = query.WhereClause;

                var visitor = new TextQueryValidatingVisitor(clauseNames.PrimaryName);

                bool ignoredResult = whereClause.Accept(visitor);
                if (visitor.Valid)
                {
                    string textQuery = visitor.GetTextTerminalValue(OperandResolver, user);
                    return textQuery != null;
                }
            }
            return false;
        }

        public override void PopulateFromParams(User user, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            fieldValuesHolder[FieldsKey] = actionParams.GetFirstValueForKey(Id);
        }

        public override void ValidateParams(User searcher, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, ModelState errors)
        {
            throw new NotImplementedException();
        }

        public override void PopulateFromQuery(User user, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext)
        {
            if (query == null || query.WhereClause == null) return;

            var whereClause = query.WhereClause;
            var visitor = new TextQueryValidatingVisitor(clauseNames.PrimaryName);

            bool ignoredResult = whereClause.Accept(visitor);
            if (!visitor.Valid) return;

            string textQuery = visitor.GetTextTerminalValue(OperandResolver, user);
            if (textQuery != null)
            {
                fieldValuesHolder[FieldsKey] = textQuery.Trim();
            }
        }

        public override IClause GetSearchClause(User user, IFieldValuesHolder fieldValuesHolder)
        {
            string query = ParameterUtils.GetStringParam(fieldValuesHolder, FieldsKey);
            if (query != null)
            {
                return new TerminalClause(clauseNames.PrimaryName, Operator.LIKE, query);
            }

            return null;
        }
    }
}