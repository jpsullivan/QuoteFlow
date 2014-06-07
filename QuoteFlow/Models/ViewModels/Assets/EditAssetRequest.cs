using System;

namespace QuoteFlow.Models.ViewModels.Assets
{
    public class EditAssetRequest
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        private decimal _cost;
        public decimal Cost
        {
            get { return decimal.Round(_cost, 2, MidpointRounding.AwayFromZero); }
            set { _cost = value; }
        }
        public decimal Markup { get; set; }
    }
}