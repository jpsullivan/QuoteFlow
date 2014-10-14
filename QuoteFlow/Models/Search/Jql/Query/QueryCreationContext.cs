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

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var that = (QueryCreationContext) obj;

            if (SecurityOverriden != that.SecurityOverriden)
            {
                return false;
            }
            if (User != null ? !User.Equals(that.User) : that.User != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = User != null ? User.GetHashCode() : 0;
            result = 31 * result + (SecurityOverriden ? 1 : 0);
            return result;
        }
    }
}