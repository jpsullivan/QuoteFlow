using System;
using System.Collections.Generic;
using System.Linq;
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
            var insert = Current.DB.QuoteStatus.Insert(new
            {
                Name = status.Name,
                OrganizationId = 1,
                OrderNum = 4
            });

            if (insert == null)
            {
                throw new InvalidOperationException("Quote status insert failed.");
            }
        }

        public void CreateStatus(string name, int organizationId)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(QuoteStatus status)
        {
            Current.DB.QuoteStatus.Update(status.Id, status);
        }

        public void UpdateStatuses(List<QuoteStatus> statuses)
        {
            // rebuild the status order
            for (int i = 0; i < statuses.Count; i++)
            {
                var status = statuses.ElementAt(i);
                status.OrderNum = i + 1;
            }

            foreach (var status in statuses)
            {
                UpdateStatus(status);
            }
        }

        public void DeleteStatus(int id)
        {
            Current.DB.QuoteStatus.Delete(id);
        }

        public void MoveStatus(int id, int movePos)
        {
            var statuses = GetStatuses(1).ToList(); // todo: remove organization id
            int indexToMove = FindIndexById(statuses, id);
            int indexToMoveAfter = FindIndexById(statuses, movePos);
            var newList = new List<QuoteStatus>();

            for (int i = 0; i < statuses.Count; i++)
            {
                if (i == indexToMove) continue;
                newList.Add(statuses.ElementAt(i));

                if (i == indexToMoveAfter)
                {
                    newList.Add(statuses.ElementAt(indexToMove));
                }
            }

            UpdateStatuses(newList);
        }

        private int FindIndexById(List<QuoteStatus> statuses, int id)
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                var status = statuses[i];
                if (status.Id == id)
                {
                    return i;
                }
            }

            throw new InvalidOperationException("Quote status not found");
        }
    }
}
