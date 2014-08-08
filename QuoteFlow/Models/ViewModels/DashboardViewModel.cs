using System.Collections.Generic;

namespace QuoteFlow.Models.ViewModels
{
    public class DashboardViewModel
    {
        public bool SkipGettingStarted { get; set; }
        public IEnumerable<Catalog> Catalogs { get; set; }
        public IEnumerable<Quote> Quotes { get; set; } 
    }
}