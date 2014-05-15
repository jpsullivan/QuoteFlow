namespace QuoteFlow.Models.CatalogImport
{
    public interface ICatalogSummaryRecord
    {
        /// <summary>
        /// The row id.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// The row line number that failed to import.
        /// </summary>
        int RowId { get; set; }

        /// <summary>
        /// The row import result.
        /// </summary>
        CatalogSummaryResult Result { get; set; }
    }
}