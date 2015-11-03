using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Services
{
    public interface IQuoteStatusService
    {
        IEnumerable<QuoteStatus> GetStatuses(int organizationId);

        void CreateStatus(QuoteStatus status);

        void CreateStatus(string name, int organizationId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        void UpdateStatus(QuoteStatus status);

        /// <summary>
        /// Batch updates a collection of statuses. This will also rebuild
        /// the order, based on the order of the "statuses" parameter.
        /// </summary>
        /// <param name="statuses">The collection of statuses to update</param>
        void UpdateStatuses(List<QuoteStatus> statuses);

        void DeleteStatus(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movePos"></param>
        void MoveStatus(int id, int movePos);
    }
}