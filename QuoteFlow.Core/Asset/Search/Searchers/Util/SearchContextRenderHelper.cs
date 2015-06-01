using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Api.Asset.Search;

namespace QuoteFlow.Core.Asset.Search.Searchers.Util
{
    /// <summary>
    /// Adds search context parameters to templateParams.
    /// </summary>
    public class SearchContextRenderHelper
    {
        public static void AddSearchContextParams(ISearchContext searchContext, IDictionary<string, object> templateParams)
        {
            var catalogNames = new List<string>();
            var contextCatalogs = searchContext.Catalogs.ToList();
            if (contextCatalogs.Any())
            {
                catalogNames.AddRange(contextCatalogs.Select(c => HttpUtility.HtmlEncode(c.Name)));
            }

            templateParams.Add("contextCatalogNames", catalogNames);

            var manufacturerNames = new List<string>();
            var contextManufacturers = searchContext.Manufacturers.ToList();
            if (contextManufacturers.Any())
            {
                manufacturerNames.AddRange(contextManufacturers.Select(m => HttpUtility.HtmlEncode(m.Name)));
            }

            templateParams.Add("contextManufacturerNames", manufacturerNames);
        }
    }
}