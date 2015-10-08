using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels.Quotes;
using QuoteFlow.Api.Services;
using StackExchange.Profiling.Helpers.Dapper;

namespace QuoteFlow.Core.Services
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
            var quote = Current.DB.Quotes.Get(quoteId);

            const string countSql = "select count(*) from QuoteLineItems where QuoteId = @id";
            int totalLineItems = Current.DB.Query<int>(countSql, new {id = quote.Id}).First();

            quote.TotalLineItems = totalLineItems;

            return quote;
        }

        /// <summary>
        /// Creates a new quote based on a <see cref="NewQuoteModel"/> ViewModel and the 
        /// current user.
        /// </summary>
        /// <param name="model">The <see cref="NewQuoteModel"/> ViewModel.</param>
        /// <param name="userId">The creator of the quote. Typically the current user.</param>
        /// <returns>The newly created <see cref="Quote"/>.</returns>
        public Quote CreateQuote(NewQuoteModel model, int userId)
        {
            var quote = new Quote();
            var insert = Current.DB.Quotes.Insert(new
            {
                Name = model.QuoteName,
                Description = model.QuoteDescription,
                Status = 1,
                Responded = false,
                CreatorId = userId,
                CustomerId = model.CustomerId,
                OrganizationId = model.Organization.Id,
                CreationDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                Enabled = true
            });
            if (insert == null)
            {
                throw new ArgumentException("Quote insert failed.");
            }

            quote = GetQuote(insert.Value);
            return quote;
        }

        /// <summary>
        /// Find all quotes for a specific customer.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns></returns>
        public IEnumerable<Quote> GetCustomerQuotes(int customerId)
        {
            if (customerId == 0)
            {
                throw new ArgumentException("Customer ID must not be zero.");
            }

            var builder = new SqlBuilder();
            var tmpl = builder.AddTemplate(@"
                select
                    *,
                    (select count(*) from QuoteLineItems where QuoteId = quotes.Id) TotalLineItems
                from
                (
                    select * from Quotes /**where**/
                ) as quotes
            ");

            builder.Where("CustomerId = @customerId");

            return Current.DB.Query<Quote>(tmpl.RawSql, new { customerId });
        }

        /// <summary>
        /// Fetch all the quotes from a particular organization
        /// </summary>
        /// <param name="organizationId" type="int">The organization id</param>
        /// <returns>List of quotes from the organization</returns>
        public IEnumerable<Quote> GetQuotesFromOrganization(int organizationId)
        {
            var builder = new SqlBuilder();
            var tmpl = builder.AddTemplate(@"
                select
                    *,
                    (select count(*) from QuoteLineItems where QuoteId = quotes.Id) TotalLineItems
                from
                (
                    select * from Quotes /**where**/
                ) as quotes
            ");

            builder.Where("OrganizationId = @organizationId");

            return Current.DB.Query<Quote>(tmpl.RawSql, new { organizationId });
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