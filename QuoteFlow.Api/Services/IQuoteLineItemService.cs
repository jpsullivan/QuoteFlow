using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Services
{
    public interface IQuoteLineItemService
    {
        /// <summary>
        /// Retrieve a single <see cref="QuoteLineItem"/> record based on its ID.
        /// </summary>
        /// <param name="id">The ID of the line item to retrieve.</param>
        /// <returns></returns>
        QuoteLineItem GetLineItem(int id);

        /// <summary>
        /// Retrieves a collection of <see cref="QuoteLineItem"/> records based on the quote ID.
        /// </summary>
        /// <param name="quoteId">The ID of the quote to retrieve line items from.</param>
        /// <returns></returns>
        IEnumerable<QuoteLineItem> GetLineItems(int quoteId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineItem"></param>
        QuoteLineItem AddLineItem(QuoteLineItem lineItem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="assetId"></param>
        /// <param name="quantity"></param>
        QuoteLineItem AddLineItem(int quoteId, int assetId, int quantity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="assetId"></param>
        /// <param name="quantity"></param>
        void UpdateLineItem(int quoteId, int assetId, int quantity);

        /// <summary>
        /// Deletes a single line item based on its ID.
        /// </summary>
        /// <param name="id">The ID of the line item to delete.</param>
        void DeleteLineItem(int id);
    }
}