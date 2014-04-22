using System.Collections.Generic;
using QuoteFlow.Models;

namespace QuoteFlow.Services.Interfaces
{
    public interface IQuoteService
    {
        /// <summary>
        /// Returns a quote object based on the quote id
        /// </summary>
        /// <param name="quoteId">The id to search for</param>
        /// <returns type="Quote">A quote object</returns>
        Quote GetQuote(int quoteId);

        /// <summary>
        /// Fetch all the quotes from a particular organization
        /// </summary>
        /// <param name="organizationId" type="int">The organization id</param>
        /// <returns>List of quotes from the organization</returns>
        IEnumerable<Quote> GetQuotesFromOrganization(int organizationId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quoteName"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        bool ExistsWithinOrganization(string quoteName, int organizationId);
    }
}