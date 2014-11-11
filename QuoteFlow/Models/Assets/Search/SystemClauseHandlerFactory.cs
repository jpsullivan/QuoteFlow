using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteFlow.Models.Assets.Search
{
    public class SystemClauseHandlerFactory : ISystemClauseHandlerFactory
    {

        public SystemClauseHandlerFactory()
        {
        }

        private IEnumerable<SearchHandler> systemClauseSearchHandlers = new List<SearchHandler>()
        {
            createSavedFilterSearchHandler(),
            createIssueKeySearchHandler(),
            createIssueParentSearchHandler(),
            createCurrentEstimateSearchHandler(),
            createOriginalEstimateSearchHandler(),
            createTimeSpentSearchHandler(),
            createSecurityLevelSearchHandler(),
            createVotesSearchHandler(),
            createVoterSearchHandler(),
            createWatchesSearchHandler(),
            createWatcherSearchHandler(),
            createProjectCategoryHandler(),
            createSubTaskSearchHandler(),
            createProgressSearchHandler(),
            createLastViewedHandler(),
            createAttachmentsSearchHandler(),
            createIssuePropertySearchHandler(),
            createStatusCategorySearchHandler()
        };

        public ICollection<SearchHandler> GetSystemClauseSearchHandlers()
        {
            
        }

        private SearchHandler CreateSavedFilterSearchHandler() { throw new NotImplementedException(); }
    }
}