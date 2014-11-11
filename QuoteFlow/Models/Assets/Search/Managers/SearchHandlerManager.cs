using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.UI;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Values;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Managers
{
    public class SearchHandlerManager : ISearchHandlerManager
    {
        private readonly ISystemClauseHandlerFactory systemClauseHandlerFactory;
        private readonly IQueryCache queryCache;
//        private readonly CachedReference<Helper> helperResettableLazyReference;

        public SearchHandlerManager(ISystemClauseHandlerFactory systemClauseHandlerFactory, IQueryCache queryCache)
        {
            if (systemClauseHandlerFactory == null)
            {
                throw new ArgumentNullException("systemClauseHandlerFactory");
            }

            this.queryCache = queryCache;
            this.systemClauseHandlerFactory = systemClauseHandlerFactory;
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetSearchers(User searcher, ISearchContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Verify();
            return toList(filter(GetAllSearchers(), new IsShown(searcher, context)));
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetAllSearchers()
        {
            // eventually filter all the searchers by whether or not they are shown
            return GetAllSearchers();
        }

        public ICollection<SearcherGroup> SearcherGroups { get; private set; }
        
        public IAssetSearcher<ISearchableField> GetSearcher(string id)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClauseHandler> GetClauseHandler(User user, string jqlClauseName)
        {
            var clauseHandler = queryCache.GetClauseHandlers(user, jqlClauseName);
            if (clauseHandler == null)
            {
                var filteredHandlers = new List<ClauseHandler>();
                var unfilteredHandlers = GetClauseHandler(jqlClauseName);
                foreach (ClauseHandler handler in unfilteredHandlers)
                {
                    if (handler.PermissionHandler.hasPermissionToUseClause(user))
                    {
                        filteredHandlers.Add(handler);
                    }
                }
                clauseHandler = new List<IClauseHandler>(filteredHandlers);
                queryCache.SetClauseHandlers(user, jqlClauseName, clauseHandler);
            }

            return clauseHandler;

        }

        public IEnumerable<IClauseHandler> GetClauseHandler(string jqlClauseName)
        {
            if(jqlClauseName.IsNullOrEmpty())
            {
                throw new ArgumentException("Clause name cannot be empty.", "jqlClauseName");
            }

            return new List<ClauseHandler>(Helper.GetSearchHandler(jqlClauseName));
        }

        public ICollection<ClauseNames> GetJqlClauseNames(string fieldId)
        {
            if (fieldId.IsNullOrEmpty())
            {
                throw new ArgumentException("Field ID cannot be empty.", "fieldId");
            }

            var clauseNames = Helper.GetJqlClauseNames(fieldId);
            return clauseNames ?? new List<ClauseNames>();
        }

        public ICollection<string> GetFieldIds(User searcher, string jqlClauseName)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetFieldIds(string jqlClauseName)
        {
            throw new NotImplementedException();
        }

        public ICollection<ClauseNames> GetVisibleJqlClauseNames(User searcher)
        {
            throw new NotImplementedException();
        }

        public ICollection<IClauseHandler> getVisibleClauseHandlers(User searcher)
        {
            throw new NotImplementedException();
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetSearchersByClauseName(User user, string jqlClauseName)
        {
            if (jqlClauseName.IsNullOrEmpty())
            {
                throw new ArgumentException("Clause name cannot be empty.", "jqlClauseName");
            }
        }

        /// <summary>
		/// The delegate used by the manager to implement its functionality in a thread safe way.
		/// </summary>
		internal class Helper
		{
			/// <summary>
			/// ClauseName -> ClauseHandler.
			/// </summary>
			internal readonly IDictionary<string, IList<IClauseHandler>> handlerIndex;

			/// <summary>
			/// SearcherId -> IssueSearcher.
			/// </summary>
			internal readonly IDictionary<string, IAssetSearcher<ISearchableField>> searcherIndex;

			/// <summary>
			/// ClauseName -> SearcherRegistration.
			/// </summary>
			internal readonly IDictionary<string, IList<SearchHandler.SearcherRegistration>> searcherClauseNameIndex;

			/// <summary>
			/// FieldId -> ClauseName.
			/// </summary>
			internal readonly IDictionary<string, IList<ClauseNames>> fieldIdToClauseNames;

			/// <summary>
			/// All JIRA's searcher groups.
			/// </summary>
			internal readonly IList<SearcherGroup> searcherGroup;

			public Helper(SearchHandlerIndexer indexer)
			{
				searcherIndex = indexer.createSearcherIdIndex();
				handlerIndex = indexer.createHandlerIndex();
				searcherGroup = indexer.createSearcherGroups();
				searcherClauseNameIndex = indexer.createSearcherJqlNameIndex();
				fieldIdToClauseNames = indexer.createFieldToClauseNamesIndex();
			}

			public virtual ICollection<IClauseHandler> GetSearchHandler(string jqlName)
			{
			    var handler = handlerIndex[jqlName.ToLower(CultureInfo.CurrentCulture)];
			    if (handler == null)
			    {
			        return new Collection<IClauseHandler>();
			    }
			    return handler;
			}

			public virtual ICollection<IClauseHandler> SearchHandlers
			{
				get
				{
					var allHandlers = new Set<IClauseHandler>();
					var handlersList = handlerIndex.Values;
					foreach (var clauseHandlers in handlersList)
					{
						allHandlers.AddMany(clauseHandlers);
					}
					return allHandlers;
				}
			}

			public virtual IssueSearcher<?> getIssueSearcher(string searcher)
			{
				return searcherIndex[searcher];
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.Collection<com.atlassian.jira.issue.search.searchers.IssueSearcher<?>> getAllIssueSearchers()
			public virtual ICollection<IssueSearcher<?>> AllIssueSearchers
			{
				get
				{
					return searcherIndex.Values;
				}
			}

			public virtual ICollection<SearchHandler.SearcherRegistration> getIssueSearcherRegistrationsByClauseName(string jqlName)
			{
				return returnNullAsEmpty(searcherClauseNameIndex[CaseFolding.foldString(jqlName)]);
			}

			public virtual IList<SearcherGroup> SearcherGroups
			{
				get
				{
					return searcherGroup;
				}
			}

			public virtual IList<ClauseNames> getJqlClauseNames(string fieldId)
			{
				return fieldIdToClauseNames[fieldId];
			}
		}
    }
}