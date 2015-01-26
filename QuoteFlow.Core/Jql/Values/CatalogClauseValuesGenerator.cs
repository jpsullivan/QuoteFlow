using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Jql.Values
{
    /// <summary>
    /// Provides the possible values for projects that the user can see.
    /// </summary>
    public class CatalogClauseValuesGenerator : IClauseValuesGenerator
    {
        public ICatalogService CatalogService { get; protected set; }

        public CatalogClauseValuesGenerator(ICatalogService catalogService)
        {
            CatalogService = catalogService;
        }

        public ClauseValueResults GetPossibleValues(User searcher, string jqlClauseName, string valuePrefix, int maxNumResults)
        {
            var catalogs = new List<Catalog>(CatalogService.GetCatalogs(1)); // todo: inject organization context
            var orderedCatalogs = catalogs.OrderBy(x => x.Name);
            
            var resultVals = new List<ClauseValueResult>();

            foreach (Catalog catalog in orderedCatalogs)
            {
                if (resultVals.Count == maxNumResults)
                {
                    break;
                }
                string lowerCaseProjName = catalog.Name.ToLower();
                if (ValueMatchesProject(valuePrefix, lowerCaseProjName))
                {
                    resultVals.Add(new ClauseValueResult(catalog.Name));
                }
            }

            return new ClauseValueResults(resultVals);
        }

        private bool ValueMatchesProject(string valuePrefix, string lowerCaseProjName)
        {
            if (string.IsNullOrWhiteSpace(valuePrefix))
            {
                return true;
            }
            if (lowerCaseProjName.StartsWith(valuePrefix.ToLower()))
            {
                return true;
            }
            return false;
        }
    }
}