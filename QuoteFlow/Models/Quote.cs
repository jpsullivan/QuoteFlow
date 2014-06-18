using System;
using Dapper;

namespace QuoteFlow.Models
{
    public class Quote
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public QuoteStatus Status { get; set; }
        public bool? Responded { get; set; }

        private decimal _totalPrice;
        [IgnoreProperty(true)]
        public decimal TotalPrice
        {
            get { return decimal.Round(_totalPrice, 2, MidpointRounding.AwayFromZero); }
            set { _totalPrice = value; }
        }

        public int CreatorId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// Enum used for determining the quote status
    /// </summary>
    public enum QuoteStatus
    {
        Pending = 1,
        POReceived = 2,
        Ordered = 3,
        Fulfilled = 4,
        Lost = 5
    }
}