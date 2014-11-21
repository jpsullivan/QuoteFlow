using System;
using System.Collections.Generic;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Assets.CustomFields.Searchers.Transformer;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Assets.Transport;
using QuoteFlow.Models.Search.Jql.Function;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    /// <summary>
    /// An search input transformer for asset type
    /// </summary>
    public class UserSearchInputTransformer : ISearchInputTransformer
    {
        //protected internal readonly UserHistoryManager userHistoryManager;
        protected readonly UserFieldSearchConstants searchConstants;
        //protected internal readonly UserFitsNavigatorHelper userFitsNavigatorHelper;
        //protected internal readonly GroupManager groupManager;
        protected readonly IUserService userService;
        protected readonly string emptySelectFlag;
        private readonly ICustomField customField;
        private readonly ICustomFieldInputHelper customFieldInputHelper;

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

        protected internal UserSearchInputTransformer(IUserService userService, string emptySelectFlag, UserFieldSearchConstants searchConstants, ICustomField customField)
        {
            //this.groupManager = groupManager;
            this.userService = userService;
            this.emptySelectFlag = emptySelectFlag;
            this.searchConstants = searchConstants;
            //this.userFitsNavigatorHelper = userFitsNavigatorHelper;
            //this.userHistoryManager = userHistoryManager;
            this.customField = customField;
            //this.customFieldInputHelper = customFieldInputHelper;
        }

        public virtual void PopulateFromParams(User searcher, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            fieldValuesHolder.Add(searchConstants.SelectUrlParameter, actionParams.GetFirstValueForKey(searchConstants.SelectUrlParameter));

            // If no user select was selected but the user field was, then assume that it's a user search
            if (actionParams.ContainsKey(searchConstants.FieldUrlParameter) && !actionParams.ContainsKey(searchConstants.SelectUrlParameter))
            {
                fieldValuesHolder.Add(searchConstants.SelectUrlParameter, searchConstants.SpecificUserSelectFlag);
            }

            fieldValuesHolder.Add(searchConstants.FieldUrlParameter, actionParams.GetFirstValueForKey(searchConstants.FieldUrlParameter));
        }

        public void ValidateParams(User searcher, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder)
        {
            string user = (string)fieldValuesHolder[searchConstants.FieldUrlParameter];
            if (user.HasValue())
            {
                string userTypeSelectList = (string)fieldValuesHolder[searchConstants.SelectUrlParameter];
                if (searchConstants.SpecificUserSelectFlag.Equals(userTypeSelectList))
                {
                    if (!UserExists(user))
                    {
                        //errors.Errors.Add(string.Format("admin.errors.could.not.find.username: {0}", user));
                    }
                }
                else if (searchConstants.SpecificGroupSelectFlag.Equals(userTypeSelectList))
                {
                    if (!GroupExists(user))
                    {
                        //errors.Errors.Add(string.Format("admin.errors.abstractusersearcher.could.not.find.group: {0}", user));
                    }
                }
            }
        }

        public virtual void PopulateFromQuery(User searcher, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext)
        {
            if (query == null)
            {
                return;
            }

            var clauses = GetMatchingClauses(searchConstants.JqlClauseNames.JqlFieldNames, query);

            foreach (var clause in clauses)
            {
                var operand = clause.Operand;
                if (operand is SingleValueOperand)
                {
                    var svop = (SingleValueOperand) operand;
                    string stringValue = svop.StringValue ?? svop.IntValue.ToString();
                    //string user = userFitsNavigatorHelper.checkUser(stringValue);
                    string user = null;
                    if (user != null)
                    {
                        fieldValuesHolder.Add(searchConstants.FieldUrlParameter, user);
                        fieldValuesHolder.Add(searchConstants.SelectUrlParameter, searchConstants.SpecificUserSelectFlag);
                    }
                }
                else if (operand is FunctionOperand)
                {
                    var fop = (FunctionOperand) operand;
                    if (MembersOfFunction.FUNCTION_MEMBERSOF.Equals(fop.Name, StringComparison.InvariantCultureIgnoreCase) && fop.Args.Count == 1)
                    {
                        string group = fop.Args[0];
                        fieldValuesHolder.Add(searchConstants.FieldUrlParameter, group);
                        fieldValuesHolder.Add(searchConstants.SelectUrlParameter, searchConstants.SpecificGroupSelectFlag);
                    }
                    else if (CurrentUserFunction.FUNCTION_CURRENT_USER.Equals(fop.Name, StringComparison.InvariantCultureIgnoreCase))
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

        public virtual bool DoRelevantClausesFitFilterForm(User searcher, IQuery query, ISearchContext searchContext)
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

            ITerminalClause clause = clauses[0];
            Operator @operator = clause.Operator;
            var operand = clause.Operand;
            if (operand is SingleValueOperand)
            {
                if (@operator != Operator.EQUALS) return false;

                var svop = (SingleValueOperand) operand;
                string user = svop.StringValue ?? svop.IntValue.ToString();
                return false;
                //return userFitsNavigatorHelper.checkUser(user) != null;
            }
            if (operand is FunctionOperand)
            {
                var fop = (FunctionOperand) operand;
                if (MembersOfFunction.FUNCTION_MEMBERSOF.Equals(fop.Name, StringComparison.InvariantCultureIgnoreCase) && fop.Args.Count == 1 && @operator == Operator.IN)
                {
                    return true;
                }
                if (CurrentUserFunction.FUNCTION_CURRENT_USER.Equals(fop.Name, StringComparison.InvariantCultureIgnoreCase) && @operator == Operator.EQUALS && IsUserLoggedIn(searcher))
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

        public virtual IClause GetSearchClause(User searcher, IFieldValuesHolder fieldValuesHolder)
        {
            string clauseName = GetClauseName(searcher);
            if (emptySelectFlag != null && ParameterUtils.ParamContains(fieldValuesHolder, searchConstants.SelectUrlParameter, emptySelectFlag))
            {
                return new TerminalClause(clauseName, Operator.IS, new EmptyOperand());
            }
            
            if (ParameterUtils.ParamContains(fieldValuesHolder, searchConstants.SelectUrlParameter, searchConstants.CurrentUserSelectFlag))
            {
                return new TerminalClause(clauseName, Operator.EQUALS, new FunctionOperand(CurrentUserFunction.FUNCTION_CURRENT_USER));
            }
            
            // We do a specific user search when either the type is specifed or (when there is no search type and there is a value).
            if (fieldValuesHolder.ContainsKey(searchConstants.FieldUrlParameter) && (!fieldValuesHolder.ContainsKey(searchConstants.SelectUrlParameter) || (ParameterUtils.ParamContains(fieldValuesHolder, searchConstants.SelectUrlParameter, searchConstants.SpecificUserSelectFlag))))
            {
                string user = ParameterUtils.GetStringParam(fieldValuesHolder, searchConstants.FieldUrlParameter);
                return new TerminalClause(clauseName, Operator.EQUALS, user);
            }
            
            if (ParameterUtils.ParamContains(fieldValuesHolder, searchConstants.SelectUrlParameter, searchConstants.SpecificGroupSelectFlag) && fieldValuesHolder.ContainsKey(searchConstants.FieldUrlParameter))
            {
                string group = ParameterUtils.GetStringParam(fieldValuesHolder, searchConstants.SelectUrlParameter, searchConstants.SpecificUserSelectFlag, searchConstants.FieldUrlParameter);
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
            return customFieldInputHelper.GetUniqueClauseName(user, searchConstants.JqlClauseNames.PrimaryName, customField.Name);
        }

        protected internal virtual IEnumerable<ITerminalClause> GetMatchingClauses(IEnumerable<string> jqlClauseNames, IQuery query)
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
            return userService.GetUser(user) != null;
        }

        internal virtual SimpleNavigatorCollectorVisitor CreateSimpleNavigatorCollectorVisitor()
        {
            return new SimpleNavigatorCollectorVisitor(searchConstants.JqlClauseNames.JqlFieldNames);
        }
    }
}