using System;
using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Search
{
    public class SystemClauseHandlerFactory : ISystemClauseHandlerFactory
    {
        public SystemClauseHandlerFactory()
        {
        }

        public IEnumerable<SearchHandler> GetSystemClauseSearchHandlers()
        {
            var systemClauseSearchHandlers = new List<SearchHandler>
            {
//                CreateSavedFilterSearchHandler(),
//                createIssueKeySearchHandler(),
//                createIssueParentSearchHandler(),
//                createCurrentEstimateSearchHandler(),
//                createOriginalEstimateSearchHandler(),
//                createTimeSpentSearchHandler(),
//                createSecurityLevelSearchHandler(),
//                createVotesSearchHandler(),
//                createVoterSearchHandler(),
//                createWatchesSearchHandler(),
//                createWatcherSearchHandler(),
//                createProjectCategoryHandler(),
//                createSubTaskSearchHandler(),
//                createProgressSearchHandler(),
//                createLastViewedHandler(),
//                createAttachmentsSearchHandler(),
//                createIssuePropertySearchHandler(),
//                createStatusCategorySearchHandler()
            };

            return systemClauseSearchHandlers;
        }

        private SearchHandler CreateSavedFilterSearchHandler() { throw new NotImplementedException(); }
    }
}