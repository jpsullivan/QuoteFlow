using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Services
{
    public interface IQuoteStatusService
    {
        IEnumerable<QuoteStatus> GetStatuses(int organizationId);

        void CreateStatus(QuoteStatus status);

        void CreateStatus(string name, int organizationId);

        void DeleteStatus(int id);
    }
}