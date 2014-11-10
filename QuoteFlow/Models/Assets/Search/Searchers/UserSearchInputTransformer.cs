using System.Collections.Generic;
using System.Web.Http.ModelBinding;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Assets.Transport;
using QuoteFlow.Models.Search.Jql.Function;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;
using QuoteFlow.Services.Interfaces;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    /// <summary>
    /// An search input transformer for asset type
    /// </summary>
    public class UserSearchInputTransformer : ISearchInputTransformer
    {
        //protected internal readonly UserHistoryManager userHistoryManager;
        protected internal readonly UserFieldSearchConstants searchConstants;
        //protected internal readonly UserFitsNavigatorHelper userFitsNavigatorHelper;
        //protected internal readonly GroupManager groupManager;
        protected internal IUserService userManager;
        protected internal readonly string emptySelectFlag;
        private readonly ICustomField customField;
        //private readonly CustomFieldInputHelper customFieldInputHelper;

        public UserSearchInputTransformer(UserFieldSearchConstantsWithEmpty searchConstants, IUserService userManager)
            : this(userManager, searchConstants.EmptySelectFlag, searchConstants, null)
        {
        }

        public UserSearchInputTransformer(UserFieldSearchConstants searchConstants, IUserService userManager)
            : this(userManager, null, searchConstants, null)
        {
        }

        public UserSearchInputTransformer(UserFieldSearchConstants searchConstants, IUserService userManager, ICustomField customField)
            : this(userManager, null, searchConstants, customField)
        {
        }

        protected internal UserSearchInputTransformer(IUserService userManager, string emptySelectFlag, UserFieldSearchConstants searchConstants, ICustomField customField)
        {
            //this.groupManager = groupManager;
            this.userManager = userManager;
            this.emptySelectFlag = emptySelectFlag;
            this.searchConstants = searchConstants;
            //this.userFitsNavigatorHelper = userFitsNavigatorHelper;
            //this.userHistoryManager = userHistoryManager;
            this.customField = customField;
            //this.customFieldInputHelper = customFieldInputHelper;
        }

        public void PopulateFromParams(User searcher, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            fieldValuesHolder.Add(searchConstants.SelectUrlParameter, actionParams.GetFirstValueForKey(searchConstants.SelectUrlParameter));

            // If no user select was selected but the user field was, then assume that it's a user search
            if (actionParams.ContainsKey(searchConstants.FieldUrlParameter) && !actionParams.ContainsKey(searchConstants.SelectUrlParameter))
            {
                fieldValuesHolder.Add(searchConstants.SelectUrlParameter, searchConstants.SpecificUserSelectFlag);
            }

            fieldValuesHolder.Add(searchConstants.FieldUrlParameter, actionParams.GetFirstValueForKey(searchConstants.FieldUrlParameter));
        }

        public void ValidateParams(User searcher, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, ModelState errors)
        {
            string user = (string) fieldValuesHolder[searchConstants.FieldUrlParameter];
            if (user.HasValue())
            {
                string userTypeSelectList = (string)fieldValuesHolder[searchConstants.SelectUrlParameter];
                if (searchConstants.SpecificUserSelectFlag.Equals(userTypeSelectList))
                {
                    if (!UserExists(user))
                    {
                        errors.Errors.Add(string.Format("admin.errors.could.not.find.username: {0}", user));
                    }
                }
                else if (searchConstants.SpecificGroupSelectFlag.Equals(userTypeSelectList))
                {
                    if (!GroupExists(user))
                    {
                        errors.Errors.Add(string.Format("admin.errors.abstractusersearcher.could.not.find.group: {0}", user));
                    }
                }
            }
        }

        public void PopulateFromQuery(User searcher, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext)
        {
            if (query == null)
            {
                return;
            }

            var clauses = GetMatchingClauses(searchConstants.JqlClauseNames.JqlFieldNames, query);

            foreach (TerminalClause clause in clauses)
            {
                var operand = clause.Operand;
                if (operand is SingleValueOperand)
                {
                    var svop = (SingleValueOperand) operand;
                    string stringValue = svop.StringValue ?? svop.IntValue.ToString();
                    string user = userFitsNavigatorHelper.checkUser(stringValue);
                    if (user != null)
                    {
                        fieldValuesHolder.Add(searchConstants.FieldUrlParameter, user);
                        fieldValuesHolder.Add(searchConstants.SelectUrlParameter, searchConstants.SpecificUserSelectFlag);
                    }
                }
                else if (operand is FunctionOperand)
                {
                    FunctionOperand fop = (FunctionOperand)operand;
                    if (MembersOfFunction.FUNCTION_MEMBERSOF.equalsIgnoreCase(fop.Name) && fop.Args.Count() == 1)
                    {
                        string group = fop.Args[0];
                        fieldValuesHolder.Add(searchConstants.FieldUrlParameter, group);
                        fieldValuesHolder.Add(searchConstants.SelectUrlParameter, searchConstants.SpecificGroupSelectFlag);
                    }
                    else if (CurrentUserFunction.FUNCTION_CURRENT_USER.EqualsIgnoreCase(fop.Name))
                    {
                        fieldValuesHolder.Add(searchConstants.SelectUrlParameter, searchConstants.CurrentUserSelectFlag);
                    }
                }
                else if (operand is EmptyOperand && emptySelectFlag != null)
                {
                    fieldValuesHolder.Add(searchConstants.SelectUrlParameter, emptySelectFlag);
                }
                else
                {
                    //log.warn("Operand '" + operand + "' cannot be processed in navigator for query '" + query + "'.");
                }
            }
        }

        public bool DoRelevantClausesFitFilterForm(User searcher, IQuery query, ISearchContext searchContext)
        {
            if (query == null || query.WhereClause == null) return true;

            SimpleNavigatorCollectorVisitor visitor = CreateSimpleNavigatorCollectorVisitor();
            var whereClause = query.WhereClause;
            whereClause.Accept(visitor);

            var clauses = visitor.Clauses;
            if (clauses.Count == 0)
            {
                return true;
            }
            if (clauses.Count != 1 || !visitor.Valid)
            {
                return false;
            }

            TerminalClause clause = clauses[0];
            Operator @operator = clause.Operator;
            var operand = clause.Operand;
            if (operand is SingleValueOperand)
            {
                if (@operator != Operator.EQUALS) return false;

                var svop = (SingleValueOperand) operand;
                string user = svop.StringValue ?? svop.IntValue.ToString();
                return userFitsNavigatorHelper.checkUser(user) != null;
            }
            if (operand is FunctionOperand)
            {
                var fop = (FunctionOperand)operand;
                if (MembersOfFunction.FUNCTION_MEMBERSOF.equalsIgnoreCase(fop.Name) && fop.Args.Count() == 1 && @operator == Operator.IN)
                {
                    return true;
                }
                if (CurrentUserFunction.FUNCTION_CURRENT_USER.equalsIgnoreCase(fop.Name) && @operator == Operator.EQUALS && IsUserLoggedIn(searcher))
                {
                    return true;
                }
                return false;
            }
            if (operand is EmptyOperand && emptySelectFlag != null && (@operator == Operator.EQUALS || @operator == Operator.IS))
            {
                return true;
            }
            return false;
        }

        public IClause GetSearchClause(User searcher, IFieldValuesHolder fieldValuesHolder)
        {
            string clauseName = GetClauseName(searcher);
            if (emptySelectFlag != null && ParameterUtils.paramContains(fieldValuesHolder, searchConstants.SelectUrlParameter, emptySelectFlag))
            {
                return new TerminalClause(clauseName, Operator.IS, new EmptyOperand());
            }
            
            if (ParameterUtils.paramContains(fieldValuesHolder, searchConstants.SelectUrlParameter, searchConstants.CurrentUserSelectFlag))
            {
                return new TerminalClause(clauseName, Operator.EQUALS, new FunctionOperand(CurrentUserFunction.FUNCTION_CURRENT_USER));
            }
            
            //We do a specific user search when either the type is specifed or (when there is no search type and there is a value).
            if (fieldValuesHolder.ContainsKey(searchConstants.FieldUrlParameter) && (!fieldValuesHolder.ContainsKey(searchConstants.SelectUrlParameter) || (ParameterUtils.paramContains(fieldValuesHolder, searchConstants.SelectUrlParameter, searchConstants.SpecificUserSelectFlag))))
            {
                string user = ParameterUtils.getStringParam(fieldValuesHolder, searchConstants.FieldUrlParameter);
                return new TerminalClause(clauseName, Operator.EQUALS, user);
            }
            if (ParameterUtils.paramContains(fieldValuesHolder, searchConstants.SelectUrlParameter, searchConstants.SpecificGroupSelectFlag) && fieldValuesHolder.containsKey(searchConstants.FieldUrlParameter))
            {
                string group = ParameterUtils.getStringParam(fieldValuesHolder, searchConstants.SelectUrlParameter, searchConstants.SpecificUserSelectFlag, searchConstants.FieldUrlParameter);
                return new TerminalClause(clauseName, Operator.IN, new FunctionOperand(MembersOfFunction.FUNCTION_MEMBERSOF, @group));
            }
            return null;
        }

        protected internal virtual string GetClauseName(User user)
        {
            if (null == customField)
            {
                return searchConstants.JqlClauseNames.PrimaryName;
            }
            return CustomFieldInputHelper.getUniqueClauseName(user, searchConstants.JqlClauseNames.PrimaryName, customField.Name);
        }

        protected internal virtual IEnumerable<TerminalClause> GetMatchingClauses(Set<string> jqlClauseNames, IQuery query)
        {
            var clauseVisitor = new NamedTerminalClauseCollectingVisitor(jqlClauseNames);
            if (query.WhereClause != null)
            {
                query.WhereClause.Accept(clauseVisitor);
                return clauseVisitor.NamedClauses;
            }
            return new List<TerminalClause>();
        }

        internal virtual bool IsUserLoggedIn(User user)
        {
            return user != null;
        }

        internal virtual bool GroupExists(string user)
        {
            return false;
            //return groupManager.groupExists(user);
        }

        internal virtual bool UserExists(string user)
        {
            return userManager.GetUser(user) != null;
        }

        internal virtual SimpleNavigatorCollectorVisitor CreateSimpleNavigatorCollectorVisitor()
        {
            return new SimpleNavigatorCollectorVisitor(searchConstants.JqlClauseNames.JqlFieldNames);
        }
    }
}