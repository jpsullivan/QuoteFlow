﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models.Assets.Search.Searchers.Util;
using QuoteFlow.Models.Assets.Transport;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Resolver;

namespace QuoteFlow.Models.Assets.Search.Searchers.Transformer
{
    /// <summary>
    /// A SearchInputTransformer that handles any field that is represented by its id in the Lucene document, and hence
    /// is represented by a select list in the Issue Navigator. May or may not also have flag functions.
    /// </summary>
    public abstract class IdIndexedSearchInputTransformer<T> : ISearchInputTransformer
    {
        private readonly ClauseNames clauseNames;
        private readonly string urlParameterName;
        protected internal readonly IFieldFlagOperandRegistry fieldFlagOperandRegistry;
        protected internal readonly IJqlOperandResolver operandResolver;
        protected internal readonly IIndexInfoResolver<T> indexInfoResolver;
        private volatile IIndexedInputHelper indexedInputHelper;
        private volatile IndexedInputHelper<T> defaultIndexedInputHelper;

        // This constructor assumes that clauseNames.getPrimaryName is the same as the urlParameterName, if they differ
        // do not use this constructor.
        public IdIndexedSearchInputTransformer(ClauseNames id, IIndexInfoResolver<T> indexInfoResolver, IJqlOperandResolver operandResolver, IFieldFlagOperandRegistry fieldFlagOperandRegistry)
            : this(id, id.PrimaryName, indexInfoResolver, operandResolver, fieldFlagOperandRegistry)
        {
        }

        public IdIndexedSearchInputTransformer(ClauseNames clauseNames, string urlParameterName, IIndexInfoResolver<T> indexInfoResolver, IJqlOperandResolver operandResolver, IFieldFlagOperandRegistry fieldFlagOperandRegistry)
        {
            this.clauseNames = clauseNames;
            this.urlParameterName = urlParameterName;
            this.fieldFlagOperandRegistry = fieldFlagOperandRegistry;
            this.operandResolver = operandResolver;
            this.indexInfoResolver = indexInfoResolver;
        }

        public virtual void PopulateFromParams(User user, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            fieldValuesHolder[urlParameterName] = actionParams.GetValuesForKey(urlParameterName);
        }

        public virtual void ValidateParams(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder)
        {
            // Currently doesn't do nudda (data entered through select lists)
        }
        
        public virtual void PopulateFromQuery(User user, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext)
        {
            ICollection<string> valuesAsStrings = IndexedInputHelper.GetAllNavigatorValuesForMatchingClauses(user, clauseNames, query);
            fieldValuesHolder[urlParameterName] = valuesAsStrings;
        }

        public virtual bool DoRelevantClausesFitFilterForm(User user, IQuery query, ISearchContext searchContext)
        {
            return true;
        }

        /// <summary>
        /// For this implementation we expect that the fieldValuesHolder will contain a list of
        /// strings or nothing at all, if not then this will throw an exception.
        /// </summary>
        /// <param name="user"> the user performing the search </param>
        /// <param name="fieldValuesHolder"> contains values populated by the searchers </param>
        /// <returns> Clause that represents the raw values. </returns>
        /// <exception cref="IllegalArgumentException"> if the value in the field values holder keyed by the
        /// searcher id is not a list that contains strings. </exception>
        public virtual IClause GetSearchClause(User user, IFieldValuesHolder fieldValuesHolder)
        {
            var constants = GetValuesFromHolder(fieldValuesHolder);
            if (constants != null && constants.Any())
            {
                return GetClauseForValues(constants);
            }
            return null;
        }

        /// <returns> the <see cref="IIndexedInputHelper"/> which might be specialised for this particular searcher </returns>
        internal abstract IIndexedInputHelper createIndexedInputHelper();

        internal IClause GetClauseForValues(IEnumerable<string> values)
        {
            // We only want to generate clauses with the name being the primary name
            return IndexedInputHelper.GetClauseForNavigatorValues(clauseNames.PrimaryName, values);
        }

        /// <summary>
        /// We should never be populating the FieldValuesHolder with anything but Strings for the searchers that use this
        /// InputTransformer, so we've added a runtime check to make sure that assumption is correct.
        /// </summary>
        /// <param name="fieldValuesHolder"> the field values holder </param>
        /// <returns> the values for this searcher as a list of Strings. Could be null. </returns>
        internal ISet<string> GetValuesFromHolder(IFieldValuesHolder fieldValuesHolder)
		{
			var list = fieldValuesHolder[urlParameterName];
			if (list == null)
			{
				return null;
			}

			foreach (object o in (IEnumerable) list)
			{
				if (!(o is string))
				{
					throw new System.ArgumentException("Why are we putting non-String values in the FieldValuesHolder for searcher '" + urlParameterName + "'???");
				}
			}

			// We have checked every element in the list is of type String, so it is safe to cast
            return new HashSet<string>((IList<string>) list);
		}

        /// <returns> the <see cref="DefaultIndexedInputHelper"/> always </returns>
        protected internal IndexedInputHelper<T> DefaultIndexedInputHelper
        {
            get
            {
                if (defaultIndexedInputHelper == null)
                {
                    defaultIndexedInputHelper = new IndexedInputHelper<T>(indexInfoResolver, operandResolver, fieldFlagOperandRegistry);
                }
                return defaultIndexedInputHelper;
            }
        }

        /// <returns>The <see cref="IIndexedInputHelper"/> which might be specialised for this particular searcher.</returns>
        protected internal IIndexedInputHelper IndexedInputHelper
        {
            get
            {
                if (indexedInputHelper == null)
                {
                    indexedInputHelper = createIndexedInputHelper();
                }
                return indexedInputHelper;
            }
        }
    }
}