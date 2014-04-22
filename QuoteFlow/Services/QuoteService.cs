using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class QuoteService : IQuoteService
    {
        /// <summary>
        /// Returns a quote object based on the quote id
        /// </summary>
        /// <param name="quoteId">The id to search for</param>
        /// <returns type="Quote">A quote object</returns>
        public Quote GetQuote(int quoteId)
        {
            return Current.DB.Quotes.Get(quoteId);
        }

        /// <summary>
        /// Fetch all the quotes from a particular organization
        /// </summary>
        /// <param name="organizationId" type="int">The organization id</param>
        /// <returns>List of quotes from the organization</returns>
        public IEnumerable<Quote> GetQuotesFromOrganization(int organizationId)
        {
            const string sql = "select * from Quotes where OrganizationId = @organizationId";
            return Current.DB.Query<Quote>(sql, new { organizationId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quoteName"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public bool ExistsWithinOrganization(string quoteName, int organizationId)
        {
            const string sql = "select * from Quotes where Name = @quoteName and OrganizationId = @organizationId";
            return Current.DB.Query<Quote>(sql, new { quoteName, organizationId }).Any();
        }
    }
}