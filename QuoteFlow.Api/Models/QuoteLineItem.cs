using System;

namespace QuoteFlow.Api.Models
{
    public class QuoteLineItem
    {
        // for materialization
        public QuoteLineItem()
        {
        }

        public QuoteLineItem(int quoteId, int assetId, int quantity)
        {
            QuoteId = quoteId;
            AssetId = assetId;
            Quantity = quantity;
            CreatedUtc = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public int QuoteId { get; set; }
        public int AssetId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedUtc { get; set; }

        public Asset Asset { get; set; }
    }
}
