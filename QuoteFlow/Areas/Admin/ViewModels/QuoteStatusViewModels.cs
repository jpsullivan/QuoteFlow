using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Areas.Admin.ViewModels
{
    public class QuoteStatusViewModels
    {
    }

    public class QuoteStatusListModel
    {
        public IEnumerable<QuoteStatus> Statuses { get; set; }

        public QuoteStatusListModel(IEnumerable<QuoteStatus> statuses)
        {
            Statuses = statuses;
        }
    }
}