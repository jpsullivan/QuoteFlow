using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Search.Jql.Function
{
    /// <summary>
    /// Performs the validation and value generation for the MembersOf function.
    /// The function takes exactly one argument which is the name of the group to retrieve the members from.
    /// The function is case-insensitive: if there are two groups named <code>jira-users</code> and <code>JIRA-USERS</code>,
    /// then a search for <code>membersOf("jira-USERS")</code> will return members from both.
    /// </summary>
    public class MembersOfFunction : AbstractJqlFunction
    {
        public const string FUNCTION_MEMBERSOF = "membersOf";
        private const int EXPECTED_ARGS = 1;

        public virtual IMessageSet Validate(User searcher, FunctionOperand functionOperand, ITerminalClause terminalClause)
        {
            //We don't do any permissions checks here. 3.x allowed users to search on groups they where not members of
            //provided they knew its name. As such, we need to allow this function to return groups that user is not a
            //member of.

            var messages = ValidateNumberOfArgs(functionOperand, EXPECTED_ARGS);
//            if (!messages.HasAnyErrors())
//            {
//                string groupName = functionOperand.Args[0];
//                Group group = getGroupsIgnoreCase(groupName);
//                if (group == null)
//                {
//                    messages.AddErrorMessage(string.Format("jira.jql.group.no.such.group: {0}, {1}", functionOperand.Name, groupName));
//                }
//            }
            return messages;
        }

        public virtual IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, FunctionOperand operand, ITerminalClause terminalClause)
        {
            if (!operand.Args.Any()) return new List<QueryLiteral>();

//            string groupName = operand.Args[0];
//            Group group = getGroupsIgnoreCase(groupName);
//
//            var usernames = new Set<QueryLiteral>();
//            if (@group != null)
//            {
//                var groups = new List<Group>(@group);
//                Set<User> users = userUtil.getAllUsersInGroups(groups);
//                if (users != null)
//                {
//                    foreach (User user in users)
//                    {
//                        usernames.Add(new QueryLiteral(operand, user.Username));
//                    }
//                }
//            }
//
//            return new List<QueryLiteral>(usernames);
            return new List<QueryLiteral>();
        }

        public virtual int MinimumNumberOfExpectedArguments
        {
            get { return EXPECTED_ARGS; }
        }

        public virtual IQuoteFlowDataType DataType
        {
            get { return QuoteFlowDataTypes.User; }
        }
    }

}