namespace QuoteFlow.Api.Models.CatalogImport
{
    /// <summary>
    /// To be used for auto-mapping the database results back.
    /// These will eventually be converted to an member of <see cref="ICatalogSummaryRecord"/>.
    /// </summary>
    public class CatalogImportSummaryRecord
    {
        public int Id { get; set; }

        public int CatalogId { get; set; }

        public string Result { get; set; }

        public int RowId { get; set; }

        public int? AssetId { get; set; }

        public string Reason { get; set; }
    }
}