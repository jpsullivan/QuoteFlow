using System;
using System.Collections.Generic;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class QuoteStatusService : IQuoteStatusService
    {
        public IEnumerable<QuoteStatus> GetStatuses(int organizationId)
        {
            if (organizationId == 0)
            {
                throw new ArgumentException("Organization ID must be greater than zero.", nameof(organizationId));
            }

            const string sql = "select * from QuoteStatus where OrganizationId = @organizationId order by OrderNum ASC";
            return Current.DB.Query<QuoteStatus>(sql, new {organizationId});
        }

        public void CreateStatus(QuoteStatus status)
        {
            throw new NotImplementedException();
        }

        public void CreateStatus(string name, int organizationId)
        {
            throw new NotImplementedException();
        }

        public void DeleteStatus(int id)
        {
            Current.DB.QuoteStatus.Delete(id);
        }
    }
}
