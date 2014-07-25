using System.Collections.Generic;
using System.Web.Mvc;

namespace QuoteFlow.Models.ViewModels.Assets
{
    public class AssetVarDropdown
    {
        public string AssetVarId { get; set; }

        public IEnumerable<SelectListItem> AssetVarNames { get; set; }
    }
}