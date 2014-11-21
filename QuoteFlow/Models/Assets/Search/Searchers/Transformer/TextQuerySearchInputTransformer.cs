using System;
using System.Web.Http.ModelBinding;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Assets.Transport;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Assets.Search.Searchers.Transformer
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

                whereClause.Accept(visitor);
                if (visitor.Valid)
                {
                    string textQuery = visitor.GetTextTerminalValue(operandResolver, user);
                    return textQuery != null;
                }
            }
            return false;
        }

        public override void PopulateFromParams(User user, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            fieldValuesHolder[fieldsKey] = actionParams.GetFirstValueForKey(id);
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

            whereClause.Accept(visitor);
            if (!visitor.Valid) return;

            string textQuery = visitor.GetTextTerminalValue(operandResolver, user);
            if (textQuery != null)
            {
                fieldValuesHolder[fieldsKey] = textQuery.Trim();
            }
        }

        public override IClause GetSearchClause(User user, IFieldValuesHolder fieldValuesHolder)
        {
            string query = ParameterUtils.GetStringParam(fieldValuesHolder, fieldsKey);
            if (query != null)
            {
                return new TerminalClause(clauseNames.PrimaryName, Operator.LIKE, query);
            }

            return null;
        }
    }
}