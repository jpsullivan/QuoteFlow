using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class QuoteLineItemService : IQuoteLineItemService
    {
        #region DI

        public IAssetService AssetService { get; protected set; }

        public QuoteLineItemService(IAssetService assetService)
        {
            AssetService = assetService;
        }

        #endregion

        /// <summary>
        /// Retrieve a single <see cref="QuoteLineItem"/> record based on its ID.
        /// </summary>
        /// <param name="id">The ID of the line item to retrieve.</param>
        /// <returns></returns>
        public QuoteLineItem GetLineItem(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("Line item ID must be greater than zero.", nameof(id));
            }

            const string sql = @"select * from QuoteLineItems q 
                left join Assets a on a.Id = q.AssetId 
                Order by q.Id";

            var data = Current.DB.Query<QuoteLineItem, Api.Models.Asset, QuoteLineItem>(sql, (lineItem, asset) =>
            {
                lineItem.Asset = asset;
                return lineItem;
            });
            return data.First();
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
                throw new ArgumentException("Quote ID must be greater than zero.", nameof(quoteId));
            }

            const string sql = @"select * from QuoteLineItems q 
                left join Assets a on a.Id = q.AssetId 
                Order by q.Id";

            return Current.DB.Query<QuoteLineItem, Api.Models.Asset, QuoteLineItem>(sql, (lineItem, asset) =>
            {
                lineItem.Asset = asset;
                return lineItem;
            });
        }

        public QuoteLineItem AddLineItem(QuoteLineItem lineItem)
        {
            if (lineItem == null)
            {
                throw new ArgumentNullException(nameof(lineItem));
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
                throw new ArgumentException("Line item ID must be greater than zero.", nameof(id));
            }

            Current.DB.QuoteLineItems.Delete(id);
        }
    }
}
