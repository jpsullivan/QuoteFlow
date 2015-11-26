using System.Collections.Generic;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Jql.Values
{
    /// <summary>
    /// Gets all the users for the specified value.
    /// </summary>
    public class UserClauseValuesGenerator : IClauseValuesGenerator
    {
        private readonly IUserService _userService;

        public UserClauseValuesGenerator(IUserService userService)
        {
            _userService = userService;
        }

        public ClauseValueResults GetPossibleValues(User searcher, string jqlClauseName, string valuePrefix, int maxNumResults)
        {
            var userValues = new List<ClauseValueResult>();
            return new ClauseValueResults(userValues);
        }
    }
}