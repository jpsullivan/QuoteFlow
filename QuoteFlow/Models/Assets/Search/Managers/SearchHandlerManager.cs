using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Ninject;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Handlers;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Services.Interfaces;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Managers
{
    public class SearchHandlerManager : ISearchHandlerManager
    {
        public ICacheService CacheService { get; protected set; }
        private readonly IFieldManager fieldManager;
        private readonly ISystemClauseHandlerFactory systemClauseHandlerFactory;
        private readonly IQueryCache queryCache;
        private readonly Lazy<Helper> helperResettableLazyReference;

        public SearchHandlerManager(ICacheService cacheService, IFieldManager fieldManager, ISystemClauseHandlerFactory systemClauseHandlerFactory, IQueryCache queryCache)
        {
            if (fieldManager == null)
            {
                throw new ArgumentNullException("fieldManager");
            }

            if (systemClauseHandlerFactory == null)
            {
                throw new ArgumentNullException("systemClauseHandlerFactory");
            }

            this.queryCache = queryCache;
            this.fieldManager = fieldManager;
            this.systemClauseHandlerFactory = systemClauseHandlerFactory;
            helperResettableLazyReference = new Lazy<Helper>(CreateHelper);
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetSearchers(User searcher, ISearchContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Verify();
            // eventually filter all the searchers by whether or not they are shown
            return GetAllSearchers();
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetAllSearchers()
        {
            // eventually filter all the searchers by whether or not they are shown
            return GetHelper().AllAssetSearchers;
        }

        public ICollection<SearcherGroup> SearcherGroups { get; private set; }
        
        public IAssetSearcher<ISearchableField> GetSearcher(string id)
        {
            if (id.IsNullOrEmpty())
            {
                throw new ArgumentException("Searcher ID cannot be empty", "id");
            }

            return GetHelper().GetAssetSearcher(id);
        }

        private Helper CreateHelper()
        {
            // We must process all the system fields first to ensure that we don't overwrite custom fields with
            // the system fields.
            var indexer = new SearchHandlerIndexer();
            var allSearchableFields = fieldManager.SystemSearchableFields;
            foreach (ISearchableField field in allSearchableFields)
            {
                indexer.IndexSystemField(field);
            }

            // index any textQuerySearchHandler which doesn't have a field, but does have a searcher
            var textQuerySearchHandlerFactory = Container.Kernel.TryGet<TextQuerySearchHandlerFactory>();
            if (textQuerySearchHandlerFactory != null)
            {
                indexer.indexSearchHandler(null, textQuerySearchHandlerFactory.CreateHandler(), true);
            }

            // Process all the system clause handlers, the JQL clause elements that are not associated with fields
            indexer.IndexSystemClauseHandlers(systemClauseHandlerFactory.GetSystemClauseSearchHandlers());

//            var customField = CustomFieldManager.CustomFieldObjects;
//            foreach (var field in customField)
//            {
//                indexer.IndexCustomField(field);
//            }

            return new Helper(indexer);
        }

        private Helper GetHelper()
        {
            return helperResettableLazyReference.Value;
        }

        public void Refresh()
        {
            throw new NotImplementedException();
            //helperResettableLazyReference.Reset();
        }

        public IEnumerable<IClauseHandler> GetClauseHandler(User user, string jqlClauseName)
        {
            var clauseHandler = queryCache.GetClauseHandlers(user, jqlClauseName);
            if (clauseHandler == null)
            {
                var filteredHandlers = new List<IClauseHandler>();
                var unfilteredHandlers = GetClauseHandler(jqlClauseName);
                foreach (var handler in unfilteredHandlers)
                {
//                    if (handler.PermissionHandler.hasPermissionToUseClause(user))
//                    {
//                        filteredHandlers.Add(handler);
//                    }
                    filteredHandlers.Add(handler);
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

            return new List<IClauseHandler>(GetHelper().GetSearchHandler(jqlClauseName));
        }

        public ICollection<ClauseNames> GetJqlClauseNames(string fieldId)
        {
            if (fieldId.IsNullOrEmpty())
            {
                throw new ArgumentException("Field ID cannot be empty.", "fieldId");
            }

            var clauseNames = GetHelper().GetJqlClauseNames(fieldId);
            return clauseNames ?? new List<ClauseNames>();
        }

        public ICollection<string> GetFieldIds(User searcher, string jqlClauseName)
        {
            var handler = GetHelper().GetSearchHandler(jqlClauseName);
            var fieldIds = handler.Select(clauseHandler => GetFieldId(clauseHandler)).ToList();
            return fieldIds;
        }

        public ICollection<string> GetFieldIds(string jqlClauseName)
        {
            var handler = GetHelper().GetSearchHandler(jqlClauseName);
            var fieldIds = (from clauseHandler in handler where HasFieldId(clauseHandler) select GetFieldId(clauseHandler)).ToList();
            return fieldIds;
        }

        public ICollection<ClauseNames> GetVisibleJqlClauseNames(User searcher)
        {
            var handlers = GetHelper().SearchHandlers;
            var clauseNames = handlers.Select(clauseHandler => GetClauseNames(clauseHandler)).ToList();
            return clauseNames;
        }

        public ICollection<IClauseHandler> getVisibleClauseHandlers(User searcher)
        {
            // typically this would evaluate a set of actually visible fields via the field manager,
            // but seeing as we are ignoring permissions for now, just return all the search handlers.
            return GetHelper().SearchHandlers;
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetSearchersByClauseName(User user, string jqlClauseName)
        {
            if (jqlClauseName.IsNullOrEmpty())
            {
                throw new ArgumentException("Clause name cannot be empty.", "jqlClauseName");
            }

            var regs = GetHelper().GetAssetSearcherRegistrationsByClauseName(jqlClauseName);
            var searchers = regs.Select(searcherRegistration => searcherRegistration.AssetSearcher).ToList();

            return searchers;
        }

        /// <summary>
		/// The delegate used by the manager to implement its functionality in a thread safe way.
		/// </summary>
		internal class Helper
		{
			/// <summary>
			/// ClauseName -> ClauseHandler.
			/// </summary>
			internal static IDictionary<string, IList<IClauseHandler>> handlerIndex;

			/// <summary>
			/// SearcherId -> AssetSearcher.
			/// </summary>
			internal readonly IDictionary<string, IAssetSearcher<ISearchableField>> searcherIndex;

			/// <summary>
			/// ClauseName -> SearcherRegistration.
			/// </summary>
			internal readonly IDictionary<string, IList<SearchHandler.SearcherRegistration>> searcherClauseNameIndex;

			/// <summary>
			/// FieldId -> ClauseName.
			/// </summary>
			internal static IDictionary<string, IList<ClauseNames>> fieldIdToClauseNames;

			/// <summary>
			/// All JIRA's searcher groups.
			/// </summary>
			internal readonly IList<SearcherGroup> searcherGroup;

			public Helper(SearchHandlerIndexer indexer)
			{
				searcherIndex = indexer.CreateSearcherIdIndex();
				handlerIndex = indexer.createHandlerIndex();
				searcherGroup = indexer.CreateSearcherGroups();
				searcherClauseNameIndex = indexer.createSearcherJqlNameIndex();
				fieldIdToClauseNames = indexer.CreateFieldToClauseNamesIndex();
			}

			public IEnumerable<IClauseHandler> GetSearchHandler(string jqlName)
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

			public virtual IAssetSearcher<ISearchableField> GetAssetSearcher(string searcher)
			{
				return searcherIndex[searcher];
			}

			public virtual ICollection<IAssetSearcher<ISearchableField>> AllAssetSearchers
			{
				get { return searcherIndex.Values; }
			}

			public virtual IEnumerable<SearchHandler.SearcherRegistration> GetAssetSearcherRegistrationsByClauseName(string jqlName)
			{
			    var index = searcherClauseNameIndex[jqlName.ToLower()];
			    return index ?? new Collection<SearchHandler.SearcherRegistration>();
			}

			public virtual IList<SearcherGroup> SearcherGroups
			{
				get { return searcherGroup; }
			}

			public IList<ClauseNames> GetJqlClauseNames(string fieldId)
			{
				return fieldIdToClauseNames[fieldId];
			}
		}

        /// <summary>
		/// Class that is used by the manager to build its state from <seealso cref="SearchHandler"/>s.
		/// </summary>
        internal class SearchHandlerIndexer
		{
			internal readonly Set<string> systemClauses = new Set<string>();
			internal readonly IDictionary<string, Set<IClauseHandler>> handlerIndex = new Dictionary<string, Set<IClauseHandler>>();
			internal readonly IDictionary<string, Set<SearchHandler.SearcherRegistration>> searcherClauseNameIndex = new Dictionary<string, Set<SearchHandler.SearcherRegistration>>();
			internal readonly IDictionary<string, IAssetSearcher<ISearchableField>> searcherIdIndex = new Dictionary<string, IAssetSearcher<ISearchableField>>();
			internal readonly IDictionary<SearcherGroupType, Set<IAssetSearcher<ISearchableField>>> groupIndex = new Dictionary<SearcherGroupType, Set<IAssetSearcher<ISearchableField>>>();

			internal SearchHandlerIndexer()
			{
				foreach (SearcherGroupType groupType in EnumExtensions.GetValues<SearcherGroupType>())
				{
					groupIndex[groupType] = new Set<IAssetSearcher<ISearchableField>>();
				}
			}

			internal virtual IDictionary<string, IList<ClauseNames>> CreateFieldToClauseNamesIndex()
			{
				var fieldToClauseNames = new Dictionary<string, IList<ClauseNames>>();
				foreach (Set<IClauseHandler> handlers in handlerIndex.Values)
				{
					foreach (var handler in handlers)
					{
						IClauseInformation information = handler.Information;
						if (information.FieldId != null)
						{
						    IList<ClauseNames> names;
                            fieldToClauseNames.TryGetValue(information.FieldId, out names);
							if (names == null)
							{
								names = new List<ClauseNames>();
								fieldToClauseNames[information.FieldId] = names;
							}

							names.Add(information.JqlClauseNames);
						}
					}
				}

				IDictionary<string, IList<ClauseNames>> returnMe = new Dictionary<string, IList<ClauseNames>>();
				foreach (var entry in fieldToClauseNames)
				{
					returnMe[entry.Key] = new List<ClauseNames>(entry.Value);
				}

				return returnMe;
			}

			internal virtual IDictionary<string, IList<IClauseHandler>> createHandlerIndex()
			{
				var tmpHandlerIndex = new Dictionary<string, IList<IClauseHandler>>();
				foreach (var entry in handlerIndex)
				{
					tmpHandlerIndex[entry.Key] = new List<IClauseHandler>(entry.Value);
				}
			    return tmpHandlerIndex;
			}

			internal virtual IDictionary<string, IList<SearchHandler.SearcherRegistration>> createSearcherJqlNameIndex()
			{
				var tmpHandlerIndex = new Dictionary<string, IList<SearchHandler.SearcherRegistration>>();
				foreach (var entry in searcherClauseNameIndex)
				{
					tmpHandlerIndex[entry.Key] = new List<SearchHandler.SearcherRegistration>(entry.Value);
				}
				return tmpHandlerIndex;
			}

			internal virtual IList<SearcherGroup> CreateSearcherGroups()
			{
				var groups = new List<SearcherGroup>();
				foreach (var entry in groupIndex)
				{
					if (entry.Value.Any())
					{
						var searcher = new List<IAssetSearcher<ISearchableField>>(entry.Value);
						searcher.Sort(SearcherComparatorFactory.GetSearcherComparator(entry.Key));
						groups.Add(new SearcherGroup(entry.Key, searcher));
					}
					else
					{
						groups.Add(new SearcherGroup(entry.Key, new List<IAssetSearcher<ISearchableField>>()));
					}
				}
				return new List<SearcherGroup>(groups);
			}

			internal virtual IDictionary<string, IAssetSearcher<ISearchableField>> CreateSearcherIdIndex()
			{
				return new Dictionary<string, IAssetSearcher<ISearchableField>>(searcherIdIndex);
			}

			internal virtual void IndexSystemField(ISearchableField systemField)
			{
				IndexSearchableField(systemField, true);
			}

			public virtual void IndexCustomField(ICustomField customField)
			{
				IndexSearchableField(customField, false);
			}

			public virtual void IndexSystemClauseHandlers(IEnumerable<SearchHandler> searchHandlers)
			{
				foreach (SearchHandler searchHandler in searchHandlers)
				{
					indexClauseHandlers(null, searchHandler.ClauseRegistrations, true);
				}
			}

			internal virtual void IndexSearchableField(ISearchableField field, bool system)
			{
				SearchHandler searchHandler = field.CreateAssociatedSearchHandler();
				if (searchHandler != null)
				{
					indexSearchHandler(field, searchHandler, system);
				}
				else
				{
//					if (log.DebugEnabled)
//					{
//						log.debug("Searchable field '" + field.Id + "' does not have a search handler, will not be searchable.");
//					}
				}
			}

			internal virtual void indexSearchHandler(ISearchableField field, SearchHandler handler, bool system)
			{
				SearchHandler.SearcherRegistration registration = handler.SearcherReg;
				if (registration != null)
				{
					IndexSearcherById(field, registration.AssetSearcher, system);
					// NOTE: you must call indexClauseHandlers first since it is populating a map of system fields, I know this sucks a bit, sorry :)
					indexClauseHandlers(field, registration.ClauseHandlers, system);
					IndexSearcherByJqlName(field, registration, system);
				}

				indexClauseHandlers(field, handler.ClauseRegistrations, system);
			}

			internal virtual void indexClauseHandlers<T>(ISearchableField field, IEnumerable<T> clauseHandlers, bool system) where T : SearchHandler.ClauseRegistration
			{
				foreach (var clauseHandler in clauseHandlers)
				{
					indexClauseHandlerByJqlName(field, clauseHandler, system);
				}
			}

			internal virtual void indexClauseHandlerByJqlName(IField field, SearchHandler.ClauseRegistration registration, bool system)
			{
				Set<string> names = GetClauseNames.Invoke(registration.Handler).JqlFieldNames;
				foreach (string name in names)
				{
					// We always want to look for a match in lowercase since that is how we cache it
					var lowerName = name.ToLower();
					//Do we already have a system clause of that name registered.
                    if (systemClauses.Contains(lowerName))
					{
						if (system)
						{
						    if (field != null)
							{
								throw new Exception(string.Format("Two system clauses are trying to register against the same JQL name. New Field = '{0}', Jql Name = '{1}'.", field.Name, name));
							}
						    throw new Exception(string.Format("Two system clauses are trying to register against the same JQL name. Clause with Jql Name = '{0}'.", name));
						}
//					    var type = ((ICustomField) field).CustomFieldType;
//					    string typeName = (type != null) ? type.Name : "Unknown Type";
//					    log.warn(string.Format("A custom field '{0} ({1})' is trying to register a clause handler against a system clause with name '{2}'. Ignoring request.", field.Name, typeName, name));
					}
					else
					{
						if (system)
						{
                            systemClauses.Add(lowerName);
						}

                        Register(lowerName, registration);
					}
				}
			}

			// NOTE: this method must be invoked after {@link IndexClauseHandlerByJqlName } has been called since the method
			// is responsible for populating the systemClauses set.
			internal virtual void IndexSearcherByJqlName(ISearchableField field, SearchHandler.SearcherRegistration searcherRegistration, bool system)
			{
				foreach (SearchHandler.ClauseRegistration clauseRegistration in searcherRegistration.ClauseHandlers)
				{
					foreach (string name in GetClauseNames.Invoke(clauseRegistration.Handler).JqlFieldNames)
					{
						// We always want to look for a match in lower-case since that is how we cache it
						var lowerName = name.ToLower();

                        if (!system && systemClauses.Contains(lowerName))
						{
//							var type = ((ICustomField) field).CustomFieldType;
//							string typeName = (type != null) ? type.Name : "Unknown Type";
//							log.warn(string.Format("A custom field '{0} ({1})' is trying to register a searcher against a system clause with name '{2}'. Ignoring request.", field.Name, typeName, name));
						}
						else
						{
                            Register(lowerName, searcherRegistration);
						}
					}
				}
			}

			internal virtual void Register(string name, SearchHandler.ClauseRegistration registration)
			{
                Set<IClauseHandler> currentHandlers;
				handlerIndex.TryGetValue(name, out currentHandlers);
				if (currentHandlers == null)
				{
					currentHandlers = new Set<IClauseHandler> {registration.Handler};
				    handlerIndex[name] = currentHandlers;
				}
				else
				{
                    currentHandlers.Add(registration.Handler);
				}
			}

			internal virtual void Register(string name, SearchHandler.SearcherRegistration searcherRegistration)
			{
			    Set<SearchHandler.SearcherRegistration> currentSearcherRegistrations;
				 searcherClauseNameIndex.TryGetValue(name, out currentSearcherRegistrations);
				if (currentSearcherRegistrations == null)
				{
				    currentSearcherRegistrations = new Set<SearchHandler.SearcherRegistration> {searcherRegistration};
				    searcherClauseNameIndex[name] = currentSearcherRegistrations;
				}
				else
				{
					currentSearcherRegistrations.Add(searcherRegistration);
				}
			}

			internal virtual void IndexSearcherById(ISearchableField field, IAssetSearcher<ISearchableField> newSearcher, bool system)
			{
				if (newSearcher == null)
				{
					return;
				}

				string searcherId = newSearcher.SearchInformation.Id;
			    IAssetSearcher<ISearchableField> currentSearcher;
			    searcherIdIndex.TryGetValue(searcherId, out currentSearcher);
				if (currentSearcher != null)
				{
					if (currentSearcher != newSearcher)
					{
						//log.debug(string.Format("Trying to register two searchers to the same id. Field = '{0}', Current searcher = '{1}', New Searcher = '{2}', SearcherId ='{3}'.", field.Name, currentSearcher, newSearcher, searcherId));
					}
				}
				else
				{
					searcherIdIndex[searcherId] = newSearcher;

					SearcherGroupType type;

					if (system)
					{
						type = newSearcher.SearchInformation.SearcherGroupType;
						if (type == null)
						{
							//log.warn(string.Format("System field '{0}' does not have a group type registered. Placing in {1} group.", field.Name, SearcherGroupType.CUSTOM));
							type = SearcherGroupType.Custom;
						}
					}
					else
					{
						SearcherGroupType givenType = newSearcher.SearchInformation.SearcherGroupType;
						if ((givenType != null) && (givenType != SearcherGroupType.Custom))
						{
//							var cfType = ((ICustomField) field).CustomFieldType;
//							string typeName = (cfType != null) ? cfType.Name : "Unknown Type";
//							log.warn(string.Format("Custom field '{0} ({1})' is trying to register itself in the '{2}' group.", field.Name, typeName, givenType));
						}
						type = SearcherGroupType.Custom;
					}

					groupIndex[type].Add(newSearcher);
				}
			}
		}

        private static readonly Func<IClauseHandler, string> GetFieldId = x => x.Information.FieldId;

        private static readonly Predicate<IClauseHandler> HasFieldId = handler => GetFieldId(handler) != null; 

        private static readonly Func<IClauseHandler, ClauseNames> GetClauseNames = x => x.Information.JqlClauseNames; 
    }
}