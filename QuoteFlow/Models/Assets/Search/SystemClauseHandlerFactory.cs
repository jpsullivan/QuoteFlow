﻿using System;
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

        public IEnumerable<SearchHandler> GetSystemClauseSearchHandlers()
        {
            var systemClauseSearchHandlers = new List<SearchHandler>
            {
                CreateSavedFilterSearchHandler(),
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