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

        public ICollection<SearchHandler> SystemClauseSearchHandlers { get; private set; }

        private SearchHandler CreateSavedFilterSearchHandler() { throw new NotImplementedException(); }
    }
}