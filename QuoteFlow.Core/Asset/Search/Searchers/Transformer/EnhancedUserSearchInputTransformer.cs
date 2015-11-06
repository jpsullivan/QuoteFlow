using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Function;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using Wintellect.PowerCollections;

namespace QuoteFlow.Core.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// A search input transformer for user/group fields with Kickass modifications.
    /// </summary>
    public class EnhancedUserSearchInputTransformer : UserSearchInputTransformer
    {

        public EnhancedUserSearchInputTransformer(UserFieldSearchConstantsWithEmpty searchConstants, IUserService userManager) : base(searchConstants, userManager)
        {
        }

        public EnhancedUserSearchInputTransformer(UserFieldSearchConstants searchConstants, IUserService userManager) : base(searchConstants, userManager)
        {
        }

        public EnhancedUserSearchInputTransformer(UserFieldSearchConstants searchConstants, IUserService userManager, ICustomField customField) : base(searchConstants, userManager, customField)
        {
        }

        protected internal EnhancedUserSearchInputTransformer(IUserService userService, string emptySelectFlag, UserFieldSearchConstants searchConstants, ICustomField customField) : base(userService, emptySelectFlag, searchConstants, customField)
        {
        }

        /// <summary>
        /// Populates a <see cref="FieldValuesHolder"/> with <see cref="ActionParams"/>.
        /// 
        /// Values that start with "user:" refer to users, values that start with
        /// "group:" refers to groups, and "empty" refers to empty values.
        /// </summary>
        public override void PopulateFromParams(User user, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            // For backwards compatability with old urls (eg assigneeSelect=issue_current_user), check selectUrlParameter
            var selectValues = actionParams.GetValuesForKey(searchConstants.SelectUrlParameter);
            if (selectValues.AnySafe())
            {
                base.PopulateFromParams(user, fieldValuesHolder, actionParams);
                return;
            }

            string paramName = searchConstants.FieldUrlParameter;
            Set<UserSearchInput> newValues = GetFromParams(actionParams, paramName);
            fieldValuesHolder.Add(paramName, newValues);

            if (actionParams.ContainsKey("check_prev_" + paramName)) // if there are no prev_ params, it prob wasn't a user-submitted form
            {
                Set<UserSearchInput> prevValues = GetFromParams(actionParams, "prev_" + paramName);
                UpdateUsedUsers(user, newValues, prevValues);
            }
        }

        private Set<UserSearchInput> GetFromParams(IActionParams actionParams, string paramName)
        {
            var @params = actionParams.GetValuesForKey(paramName);
            var values = new Set<UserSearchInput>();

            if (@params == null) return values;

            foreach (string param in @params)
            {
                string[] parts = param.Split(new []{ ':' }, 2);
                if (parts[0].Equals("empty"))
                {
                    values.Add(UserSearchInput.empty());
                }
                else if (parts[0].Equals("group"))
                {
                    values.Add(UserSearchInput.@group(parts[1]));
                }
                else if (parts[0].Equals("issue_current_user"))
                {
                    values.Add(UserSearchInput.currentUser());
                }
                else if (parts[0].Equals("user"))
                {
                    values.Add(UserSearchInput.user(parts[1]));
                }
            }
            
            return values;
        }

        private void UpdateUsedUsers(User remoteUser, Set<UserSearchInput> newValues, Set<UserSearchInput> prevValues)
        {
//            foreach (UserSearchInput input in Sets.difference(newValues, prevValues))
//            {
//                if (input.User)
//                {
//                    User user = userService.GetUser(input.Value);
//                    if (user != null)
//                    {
//                        userHistoryManager.addUserToHistory(UserHistoryItem.USED_USER, remoteUser, user);
//                    }
//                }
//            }
        }

        public override void ValidateParams(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder)
        {
            // nothing to do here
        }

        /// <summary>
        /// Populates a <see cref="FieldValuesHolder"/> from a <see cref="IQuery"/>.
        /// </summary>
        public override void PopulateFromQuery(User user, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext)
        {
            if (query == null)
            {
                return;
            }

            var clauses = GetMatchingClauses(searchConstants.JqlClauseNames.JqlFieldNames, query);
            var values = new Set<UserSearchInput>();

            foreach (ITerminalClause clause in clauses)
            {
                parseOperand(clause.Operand, values);
            }

            fieldValuesHolder.Add(searchConstants.FieldUrlParameter, values);
        }

        /// <summary>
        /// Convert the user's input into a JQL clause. Always in the form:
        /// 
        ///    field IN (user1, user2, membersOf(group1), membersOf(group2), ...)
        /// </summary>
        public override IClause GetSearchClause(User user, IFieldValuesHolder fieldValuesHolder)
        {
            if (fieldValuesHolder.ContainsKey(searchConstants.SelectUrlParameter))
            {
                return base.GetSearchClause(user, fieldValuesHolder);
            }
            ICollection<IOperand> operands = new List<IOperand>();
            var values = (ICollection<UserSearchInput>)fieldValuesHolder[searchConstants.FieldUrlParameter];

            if (values != null)
            {
                foreach (UserSearchInput value in values)
                {
                    if (value.CurrentUser)
                    {
                        string name = CurrentUserFunction.FUNCTION_CURRENT_USER;
                        operands.Add(new FunctionOperand(name));
                    }
                    else if (value.Empty)
                    {
                        operands.Add(EmptyOperand.Empty);
                    }
                    else if (value.Group)
                    {
                        string name = MembersOfFunction.FUNCTION_MEMBERSOF;
                        operands.Add(new FunctionOperand(name, value.Value));
                    }
                    if (value.User)
                    {
                        operands.Add(new SingleValueOperand(value.Value));
                    }
                }
            }

            if (operands.Count > 0)
            {
                string clauseName = GetClauseName(user);
                return new TerminalClause(clauseName, Operator.IN, new MultiValueOperand(operands));
            }

            return null;
        }

        /// <summary>
        /// Determines whether the given query can be represented in basic mode.
        /// </summary>
        public override bool DoRelevantClausesFitFilterForm(User user, IQuery query, ISearchContext searchContext)
        {
            if (query == null || query.WhereClause == null)
            {
                return true;
            }

            SimpleNavigatorCollectorVisitor visitor = CreateSimpleNavigatorCollectorVisitor();
            IClause whereClause = query.WhereClause;
            whereClause.Accept(visitor);

            // If we have multiple terminal clauses or the visitor determines that
            // this query can't be represented in the old navigator, it's bad!
            var clauses = visitor.Clauses;
            if (clauses.Count == 0)
            {
                return true;
            }
            if (clauses.Count > 1 || !visitor.Valid)
            {
                return false;
            }

            return CheckClause(clauses[0], user);
        }

        /// <summary>
        /// Checks whether a <see cref="TerminalClause"/> can be represented in basic mode.
        /// </summary>
        /// <param name="clause"> The clause. </param>
        /// <param name="user"> The user executing the search. </param>
        private bool CheckClause(ITerminalClause clause, User user)
        {
            Operator @operator = clause.Operator;
            IOperand operand = clause.Operand;

            if (operand is SingleValueOperand)
            {
                return CheckSingleValueClause(@operator, (SingleValueOperand)operand);
            }
            if (operand is FunctionOperand)
            {
                return CheckFunctionClause(@operator, (FunctionOperand)operand, user);
            }
            if (operand is MultiValueOperand)
            {
                return CheckMultiValueClause(@operator, (MultiValueOperand)operand, user);
            }
            if (operand is EmptyOperand && emptySelectFlag != null && (@operator == Operator.EQUALS || @operator == Operator.IS))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether a <see cref="TerminalClause"/> with a <see cref="SingleValueOperand"/>
        /// can be represented in basic mode.
        /// </summary>
        private bool CheckSingleValueClause(Operator @operator, SingleValueOperand operand)
        {
            string value = operand.StringValue;
            if (value == null)
            {
                value = operand.IntValue.ToString();
            }

            //For it to fit the operator it must be equals, the user must be a username not a fullname or email and the user
            //must exist.
            //return @operator == Operator.EQUALS && userFitsNavigatorHelper.checkUser(value) != null && userService.GetUser(value) != null;
            return @operator == Operator.EQUALS && userService.GetUser(value) != null;
        }

        /// <summary>
        /// Checks whether a <see cref="TerminalClause"/> with a <see cref="FunctionOperand"/>
        /// can be represented in basic mode.
        /// </summary>
        private bool CheckFunctionClause(Operator @operator, FunctionOperand operand, User user)
        {
            if (@operator == Operator.EQUALS && IsCurrentUser(operand) && IsUserLoggedIn(user))
            {
                // field = currentUser()
                return true;
            }
            if (@operator == Operator.IN && IsMembersOf(operand))
            {
                // field IN membersOf(group)
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether a <see cref="TerminalClause"/> with a <see cref="MultiValueOperand"/>
        /// can be represented in basic mode.
        /// </summary>
        private bool CheckMultiValueClause(Operator @operator, MultiValueOperand operand, User user)
        {
            if (@operator != Operator.IN)
            {
                return false;
            }
            foreach (IOperand value in operand.Values)
            {
                if (value is SingleValueOperand)
                {
                    // We just want to validate the user, so pass EQUALS.
                    if (!CheckSingleValueClause(Operator.EQUALS, (SingleValueOperand)value))
                    {
                        return false;
                    }
                }
                else if (value is FunctionOperand)
                {
                    bool isCurrentUser = IsUserLoggedIn(user) && IsCurrentUser((FunctionOperand)value);

                    if (!isCurrentUser && !IsMembersOf((FunctionOperand)value))
                    {
                        return false;
                    }
                }
                else if (!(value is EmptyOperand))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Extract the values (i.e. group/user) referenced in the operand.
        /// </summary>
        /// <param name="operand"> The operand from which values are to be extracted. </param>
        /// <param name="values"> The collection to add the extracted values to. </param>
        private void parseOperand(IOperand operand, ICollection<UserSearchInput> values)
        {
            if (operand is EmptyOperand)
            {
                values.Add(UserSearchInput.empty());
            }
            else if (operand is SingleValueOperand)
            {
                ParseSingleValueOperand((SingleValueOperand) operand, values);
            }
            else if (operand is FunctionOperand)
            {
                ParseFunctionOperand((FunctionOperand) operand, values);
            }
            else if (operand is MultiValueOperand)
            {
                var multiValueOperand = (MultiValueOperand) operand;
                foreach (var value in multiValueOperand.Values)
                {
                    parseOperand(value, values);
                }
            }
        }

        /// <summary>
        /// Extract the value (e.g. user) referenced in a <see cref="SingleValueOperand"/>.
        /// </summary>
        /// <param name="operand"> The operand from which the value is to be extracted. </param>
        /// <param name="values"> The collection to add the extracted value to. </param>
        private void ParseSingleValueOperand(SingleValueOperand operand, ICollection<UserSearchInput> values)
        {
            string value = operand.StringValue;
            if (value == null)
            {
                value = operand.IntValue.ToString();
            }

//            value = userFitsNavigatorHelper.checkUser(value);
//            if (value != null)
//            {
//                values.Add(UserSearchInput.user(value));
//            }
        }

        /// <summary>
        /// Extract the value (e.g. group/user) referenced in a
        /// <see cref="FunctionOperand"/>.
        /// </summary>
        /// <param name="operand"> The operand from which the value is extracted. </param>
        /// <param name="values"> The collection to add the extracted value to. </param>
        private void ParseFunctionOperand(FunctionOperand operand, ICollection<UserSearchInput> values)
        {
            if (IsCurrentUser(operand))
            {
                values.Add(UserSearchInput.currentUser());
            }
            else if (IsMembersOf(operand))
            {
                values.Add(UserSearchInput.group(operand.Args[0]));
            }
        }

        /// <summary>
        /// Determines whether a <see cref="FunctionOperand"/> represents a valid
        /// "currentUser()" function.
        /// </summary>
        private bool IsCurrentUser(FunctionOperand operand)
        {
            const string name = CurrentUserFunction.FUNCTION_CURRENT_USER;
            return operand.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether a <see cref="FunctionOperand"/> represents a valid
        /// "membersOf(group)" function.
        /// </summary>
        private bool IsMembersOf(FunctionOperand operand)
        {
            const string name = MembersOfFunction.FUNCTION_MEMBERSOF;
            return operand.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && operand.Args.Count() == 1;
        }
    }
}