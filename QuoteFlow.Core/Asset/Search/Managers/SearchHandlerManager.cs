﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Ninject;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Asset.Search.Handlers;
using QuoteFlow.Core.Asset.Search.Searchers;
using QuoteFlow.Core.DependencyResolution;
using Wintellect.PowerCollections;

namespace QuoteFlow.Core.Asset.Search.Managers
{
    public class SearchHandlerManager : ISearchHandlerManager
    {
        private readonly IFieldManager _fieldManager;
        private readonly ISystemClauseHandlerFactory _systemClauseHandlerFactory;
        private readonly IQueryCache _queryCache;
        private readonly Lazy<Helper> _helperResettableLazyReference;

        public SearchHandlerManager(IFieldManager fieldManager, ISystemClauseHandlerFactory systemClauseHandlerFactory, IQueryCache queryCache)
        {
            if (fieldManager == null)
            {
                throw new ArgumentNullException(nameof(fieldManager));
            }

            if (systemClauseHandlerFactory == null)
            {
                throw new ArgumentNullException(nameof(systemClauseHandlerFactory));
            }

            _queryCache = queryCache;
            _fieldManager = fieldManager;
            _systemClauseHandlerFactory = systemClauseHandlerFactory;
            _helperResettableLazyReference = new Lazy<Helper>(CreateHelper);
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetSearchers(User searcher, ISearchContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
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
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Searcher ID cannot be empty", nameof(id));
            }

            return GetHelper().GetAssetSearcher(id);
        }

        private Helper CreateHelper()
        {
            // We must process all the system fields first to ensure that we don't overwrite custom fields with
            // the system fields.
            var indexer = new SearchHandlerIndexer();
            var allSearchableFields = _fieldManager.SystemSearchableFields;
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
            indexer.IndexSystemClauseHandlers(_systemClauseHandlerFactory.GetSystemClauseSearchHandlers());

//            var customField = CustomFieldManager.CustomFieldObjects;
//            foreach (var field in customField)
//            {
//                indexer.IndexCustomField(field);
//            }

            return new Helper(indexer);
        }

        private Helper GetHelper()
        {
            return _helperResettableLazyReference.Value;
        }

        public void Refresh()
        {
            throw new NotImplementedException();
            //helperResettableLazyReference.Reset();
        }

        public IEnumerable<IClauseHandler> GetClauseHandler(User user, string jqlClauseName)
        {
            var clauseHandler = _queryCache.GetClauseHandlers(user, jqlClauseName);
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
                _queryCache.SetClauseHandlers(user, jqlClauseName, clauseHandler);
            }

            return clauseHandler;

        }

        public IEnumerable<IClauseHandler> GetClauseHandler(string jqlClauseName)
        {
            if (string.IsNullOrEmpty(jqlClauseName))
            {
                throw new ArgumentException("Clause name cannot be empty.", nameof(jqlClauseName));
            }

            return new List<IClauseHandler>(GetHelper().GetSearchHandler(jqlClauseName));
        }

        public ICollection<ClauseNames> GetJqlClauseNames(string fieldId)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                throw new ArgumentException("Field ID cannot be empty.", nameof(fieldId));
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
            var fieldIds = new List<string>();
            foreach (var clauseHandler in handler)
            {
                if (HasFieldId(clauseHandler))
                {
                    fieldIds.Add(GetFieldId(clauseHandler));
                }
            }
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
            if (string.IsNullOrEmpty(jqlClauseName))
            {
                throw new ArgumentException("Clause name cannot be empty.", nameof(jqlClauseName));
            }

            var regs = GetHelper().GetAssetSearcherRegistrationsByClauseName(jqlClauseName);
            var searchers = regs.Select(searcherRegistration => searcherRegistration.AssetSearcher).ToList();

            return searchers;
        }

        /// <summary>
		/// The delegate used by the manager to implement its functionality in a thread safe way.
		/// </summary>
        private class Helper
		{
			/// <summary>
			/// ClauseName -> ClauseHandler.
			/// </summary>
			private static IDictionary<string, IList<IClauseHandler>> handlerIndex;

			/// <summary>
			/// SearcherId -> AssetSearcher.
			/// </summary>
			private readonly IDictionary<string, IAssetSearcher<ISearchableField>> searcherIndex;

			/// <summary>
			/// ClauseName -> SearcherRegistration.
			/// </summary>
			private readonly IDictionary<string, IList<SearchHandler.SearcherRegistration>> searcherClauseNameIndex;

			/// <summary>
			/// FieldId -> ClauseName.
			/// </summary>
			private static IDictionary<string, IList<ClauseNames>> fieldIdToClauseNames;

			/// <summary>
			/// All QuoteFlow's searcher groups.
			/// </summary>
			private readonly IList<SearcherGroup> searcherGroup;

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
			    IList<IClauseHandler> handlers;
                if (handlerIndex.TryGetValue(jqlName.ToLower(CultureInfo.CurrentCulture), out handlers))
                {
                    return handlers ?? new List<IClauseHandler>();
                }

			    return new List<IClauseHandler>();
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
		/// Class that is used by the manager to build its state from <see cref="SearchHandler"/>s.
		/// </summary>
        private class SearchHandlerIndexer
		{
            private readonly Set<string> systemClauses = new Set<string>();
            private readonly IDictionary<string, Set<IClauseHandler>> handlerIndex = new Dictionary<string, Set<IClauseHandler>>();
            private readonly IDictionary<string, Set<SearchHandler.SearcherRegistration>> searcherClauseNameIndex = new Dictionary<string, Set<SearchHandler.SearcherRegistration>>();
            private readonly IDictionary<string, IAssetSearcher<ISearchableField>> searcherIdIndex = new Dictionary<string, IAssetSearcher<ISearchableField>>();
            private readonly IDictionary<SearcherGroupType, Set<IAssetSearcher<ISearchableField>>> groupIndex = new Dictionary<SearcherGroupType, Set<IAssetSearcher<ISearchableField>>>();

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

            protected virtual void IndexSearchableField(ISearchableField field, bool system)
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

            protected virtual void indexClauseHandlers<T>(ISearchableField field, IEnumerable<T> clauseHandlers, bool system) where T : SearchHandler.ClauseRegistration
			{
				foreach (var clauseHandler in clauseHandlers)
				{
					indexClauseHandlerByJqlName(field, clauseHandler, system);
				}
			}

            protected virtual void indexClauseHandlerByJqlName(IField field, SearchHandler.ClauseRegistration registration, bool system)
			{
				List<string> names = GetClauseNames.Invoke(registration.Handler).JqlFieldNames;
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
								throw new Exception(
								    $"Two system clauses are trying to register against the same JQL name. New Field = '{field.Name}', Jql Name = '{name}'.");
							}
						    throw new Exception(
						        $"Two system clauses are trying to register against the same JQL name. Clause with Jql Name = '{name}'.");
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
            protected virtual void IndexSearcherByJqlName(ISearchableField field, SearchHandler.SearcherRegistration searcherRegistration, bool system)
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

            protected virtual void Register(string name, SearchHandler.ClauseRegistration registration)
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

            protected virtual void Register(string name, SearchHandler.SearcherRegistration searcherRegistration)
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

            protected virtual void IndexSearcherById(ISearchableField field, IAssetSearcher<ISearchableField> newSearcher, bool system)
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