using System;
using System.Collections.Generic;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class QuoteLineItemService : IQuoteLineItemService
    {
        /// <summary>
        /// Retrieve a single <see cref="QuoteLineItem"/> record based on its ID.
        /// </summary>
        /// <param name="id">The ID of the line item to retrieve.</param>
        /// <returns></returns>
        public QuoteLineItem GetLineItem(int id)
        {
            return Current.DB.QuoteLineItems.Get(id);
        }

        /// <summary>
        /// Retrieves a collection of <see cref="QuoteLineItem"/> records based on the quote ID.
        /// </summary>
        /// <param name="quoteId">The ID of the quote to retrieve line items from.</param>
        /// <returns></returns>
        public IEnumerable<QuoteLineItem> GetLineItems(int quoteId)
        {
            if (quoteId == 0)
            {
                throw new ArgumentException("Quote ID must be greater than zero.", "quoteId");
            }

            const string sql = "select * from QuoteLineItems where QuoteId = @quoteId";
            return Current.DB.Query<QuoteLineItem>(sql, new { quoteId });
        }

        public QuoteLineItem AddLineItem(QuoteLineItem lineItem)
        {
            if (lineItem == null)
            {
                throw new ArgumentNullException("lineItem");
            }

            AddLineItem(lineItem.QuoteId, lineItem.AssetId, lineItem.Quantity);


            var insert = Current.DB.QuoteLineItems.Insert(lineItem);

            if (insert != null)
            {
                lineItem.Id = insert.Value;
            }

            return lineItem;
        }

        public QuoteLineItem AddLineItem(int quoteId, int assetId, int quantity)
        {
            var lineItem = new QuoteLineItem(quoteId, assetId, quantity);
            return AddLineItem(lineItem);
        }

        public void UpdateLineItem(int quoteId, int assetId, int quantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a single line item based on its ID.
        /// </summary>
        /// <param name="id">The ID of the line item to delete.</param>
        public void DeleteLineItem(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("Line item ID must be greater than zero.", "id");
            }

            Current.DB.QuoteLineItems.Delete(id);
        }
    }
}
