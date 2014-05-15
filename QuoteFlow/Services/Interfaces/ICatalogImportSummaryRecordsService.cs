using System.Collections.Generic;
using QuoteFlow.Models.CatalogImport;

namespace QuoteFlow.Services.Interfaces
{
    public interface ICatalogImportSummaryRecordsService
    {
        /// <summary>
        /// Fetches a collection of import summary summaries.
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        IEnumerable<ICatalogSummaryRecord> GetImportSummaryRecords(int catalogId);

        /// <summary>
        /// Converts a list of raw <see cref="CatalogImportSummaryRecord"/>s to
        /// a generic collection of <see cref="ICatalogSummaryRecord"/> objects.
        /// </summary>
        /// <param name="summaries"></param>
        /// <returns></returns>
        IEnumerable<ICatalogSummaryRecord> ConvertRawSummaryRecords(IEnumerable<CatalogImportSummaryRecord> summaries);

        /// <summary>
        /// Converts a list of resolved summaries to a collection of
        /// raw <see cref="CatalogImportSummaryRecord"/>s.
        /// </summary>
        /// <param name="summaries"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        IEnumerable<CatalogImportSummaryRecord> ConvertToRawSummaryRecords(IEnumerable<ICatalogSummaryRecord> summaries, int catalogId);

        /// <summary>
        /// Inserts a collection of raw <see cref="CatalogImportSummaryRecord"/>s 
        /// to the database.
        /// </summary>
        /// <param name="summaries">The collection of summaries to import.</param>
        void InsertSummaries(IEnumerable<CatalogImportSummaryRecord> summaries);

        /// <summary>
        /// Inserts a collection of members of <see cref="ICatalogSummaryRecord"/>
        /// where they are then resolved into raw <see cref="CatalogImportSummaryRecord"/>s.
        /// </summary>
        /// <param name="summaries">The collection of summaries to import.</param>
        /// <param name="catalogId"></param>
        void InsertSummaries(IEnumerable<ICatalogSummaryRecord> summaries, int catalogId);
    }
}