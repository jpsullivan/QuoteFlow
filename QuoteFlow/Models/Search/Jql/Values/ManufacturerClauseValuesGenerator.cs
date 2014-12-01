using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Search.Jql.Values
{
    public class ManufacturerClauseValuesGenerator : IClauseValuesGenerator
    {
        public IManufacturerService ManufacturerService { get; protected set; }

        public ManufacturerClauseValuesGenerator(IManufacturerService manufacturerService)
        {
            ManufacturerService = manufacturerService;
        }

        public ClauseValueResults GetPossibleValues(User searcher, string jqlClauseName, string valuePrefix, int maxNumResults)
        {
            var manufacturers = new List<Manufacturer>(ManufacturerService.GetManufacturers(1)); // todo: inject organization context
            var orderedManufacturers = manufacturers.OrderBy(x => x.Name);

            var resultVals = new List<ClauseValueResult>();

            foreach (Manufacturer manufacturer in orderedManufacturers)
            {
                if (resultVals.Count == maxNumResults)
                {
                    break;
                }
                string lowerCaseProjName = manufacturer.Name.ToLower();
                if (ValueMatchesManufacturer(valuePrefix, lowerCaseProjName))
                {
                    resultVals.Add(new ClauseValueResult(manufacturer.Name));
                }
            }

            return new ClauseValueResults(resultVals);
        }

        private bool ValueMatchesManufacturer(string valuePrefix, string lowerCaseProjName)
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