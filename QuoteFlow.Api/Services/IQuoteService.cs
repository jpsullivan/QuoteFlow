using System.Collections.Generic;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels.Quotes;

namespace QuoteFlow.Api.Services
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
        /// Creates a new quote based on a <see cref="NewQuoteModel"/> ViewModel and the 
        /// current user.
        /// </summary>
        /// <param name="model">The <see cref="NewQuoteModel"/> ViewModel.</param>
        /// <param name="userId">The creator of the quote. Typically the current user.</param>
        /// <returns></returns>
        Quote CreateQuote(NewQuoteModel model, int userId);

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