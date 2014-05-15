using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuoteFlow.Models.CatalogImport;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class CatalogImportSummaryRecordsService : ICatalogImportSummaryRecordsService
    {
        /// <summary>
        /// Fetches a collection of import summary summaries.
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public IEnumerable<ICatalogSummaryRecord> GetImportSummaryRecords(int catalogId)
        {
            if (catalogId == 0) {
                throw new ArgumentException("Catalog id must be greater than zero.", "catalogId");
            }

            const string sql = "select * from CatalogImportSummaryRecords where CatalogId = @catalogId";
            var records = Current.DB.Query<CatalogImportSummaryRecord>(sql, new {catalogId});
            return ConvertRawSummaryRecords(records);
        }

        /// <summary>
        /// Converts a list of raw <see cref="CatalogImportSummaryRecord"/>s to
        /// a generic collection of <see cref="ICatalogSummaryRecord"/> resolved objects.
        /// </summary>
        /// <param name="summaries"></param>
        /// <returns></returns>
        public IEnumerable<ICatalogSummaryRecord> ConvertRawSummaryRecords(IEnumerable<CatalogImportSummaryRecord> summaries)
        {
            if (summaries == null) {
                return null;
            }

            var resolvedSummaries = new List<ICatalogSummaryRecord>();

            foreach (var summary in summaries) {
                CatalogSummaryResult result;
                if (!Enum.TryParse(summary.Result, out result)) {
                    throw new Exception("No type could be parsed from the summary result.");
                }

                ICatalogSummaryRecord resolvedSummary;

                switch (result) {
                    case CatalogSummaryResult.Failure:
                        resolvedSummary = new CatalogRecordImportFailure(summary.RowId, summary.Reason);
                        break;
                    case CatalogSummaryResult.Skip:
                        resolvedSummary = new CatalogRecordImportSkipped(summary.RowId);
                        break;
                    case CatalogSummaryResult.Success:
                        resolvedSummary = new CatalogRecordImportSuccess(summary.RowId, (int) summary.AssetId);
                        break;
                    default:
                        resolvedSummary = null;
                        break;
                }

                resolvedSummaries.Add(resolvedSummary);
            }

            return resolvedSummaries;
        }

        /// <summary>
        /// Converts a list of resolved summaries to a collection of
        /// raw <see cref="CatalogImportSummaryRecord"/>s.
        /// </summary>
        /// <param name="summaries"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public IEnumerable<CatalogImportSummaryRecord> ConvertToRawSummaryRecords(IEnumerable<ICatalogSummaryRecord> summaries, int catalogId)
        {
            if (summaries == null) {
                throw new ArgumentNullException("summaries");
            }

            var rawSummaries = new List<CatalogImportSummaryRecord>();

            foreach (var summary in summaries) {
                var raw = new CatalogImportSummaryRecord();
                raw.Id = summary.Id;
                raw.CatalogId = catalogId;
                raw.Result = Enum.GetName(typeof(CatalogSummaryResult), summary.Result);
                raw.RowId = summary.RowId;

                // attempt to cast the summary as a success record to get the asset id
                var resolvedSuccess = summary as CatalogRecordImportSuccess;
                raw.AssetId = resolvedSuccess == null ? (int?) null : resolvedSuccess.AssetId;

                // attempt to cast the summary as a failure record to get the failure reason
                var resolvedFailure = summary as CatalogRecordImportFailure;
                raw.Reason = resolvedFailure == null ? null : resolvedFailure.Reason;

                rawSummaries.Add(raw);
            }

            return rawSummaries;
        }

        /// <summary>
        /// Inserts a collection of raw <see cref="CatalogImportSummaryRecord"/>s 
        /// to the database.
        /// </summary>
        /// <param name="summaries">The collection of summaries to import.</param>
        public void InsertSummaries(IEnumerable<CatalogImportSummaryRecord> summaries)
        {
            if (summaries == null) {
                throw new ArgumentNullException("summaries");
            }

            Current.DB.BeginTransaction();

            foreach (var summary in summaries) {
                Current.DB.CatalogImportSummaryRecords.Insert(summary);
            }

            Current.DB.CommitTransaction();
        }

        /// <summary>
        /// Inserts a collection of members of <see cref="ICatalogSummaryRecord"/>
        /// where they are then resolved into raw <see cref="CatalogImportSummaryRecord"/>s.
        /// </summary>
        /// <param name="summaries">The collection of summaries to import.</param>
        /// <param name="catalogId"></param>
        public void InsertSummaries(IEnumerable<ICatalogSummaryRecord> summaries, int catalogId)
        {
            if (summaries == null) {
                throw new ArgumentNullException("summaries");
            }

            var rawSummaries = ConvertToRawSummaryRecords(summaries, catalogId);
            InsertSummaries(rawSummaries);
        }
    }
}