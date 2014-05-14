using System.Collections.Generic;

namespace QuoteFlow.Models.CatalogImport
{
    public class CatalogImportSummary
    {
        IEnumerable<ICatalogSummaryRecord> SummaryRecords { get; set; }
    }
}