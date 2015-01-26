using System.Collections.Generic;

namespace QuoteFlow.Api.Models.CatalogImport
{
    public class CatalogImportSummary
    {
        IEnumerable<ICatalogSummaryRecord> SummaryRecords { get; set; }
    }
}