using System.Collections.Generic;

namespace QuoteFlow.Models.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<Catalog> Catalogs { get; set; }
        public IEnumerable<Quote> Quotes { get; set; } 
    }
}