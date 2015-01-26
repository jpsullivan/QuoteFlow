using System.Collections.Generic;

namespace QuoteFlow.Api.Models.RequestModels
{
    public class AssetCriteriaQuery
    {
        public string Decorator { get; set; }

        public string JqlContext { get; set; }

        Dictionary<string, string> Params { get; set; }
    }
}