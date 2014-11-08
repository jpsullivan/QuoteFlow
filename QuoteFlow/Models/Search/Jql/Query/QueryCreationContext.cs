namespace QuoteFlow.Models.Search.Jql.Query
{
    public class QueryCreationContext : IQueryCreationContext
    {
        /// <summary>
		/// Use this constructor if you need to override security.
		/// </summary>
		/// <param name="user"> the user performing the search </param>
		/// <param name="securityOverriden"> true if you want to override security; false otherwise </param>
		public QueryCreationContext(User user, bool securityOverriden = false)
		{
			User = user;
			SecurityOverriden = securityOverriden;
		}

        public User User { get; set; }

        public bool SecurityOverriden { get; set; }
    }
}