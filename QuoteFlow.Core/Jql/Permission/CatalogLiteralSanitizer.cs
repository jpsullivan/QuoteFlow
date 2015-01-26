using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Permission
{
    /// <summary>
    /// Sanitise the project keys, names or ids stored in <see cref="QueryLiteral"/>s.
    /// The strategy is to sanitise only those projects which both exist and the user does 
    /// not have permission to browse. The sanitised form of the operand replaces the name 
    /// or key form with the id representation.
    /// </summary>
    public class CatalogLiteralSanitizer : ILiteralSanitizer
    {
        private readonly INameResolver<Catalog> _catalogResolver;
		private readonly IIndexInfoResolver<Catalog> _catalogIndexInfoResolver;
		private readonly User user;

		public CatalogLiteralSanitizer(INameResolver<Catalog> catalogResolver, User user) 
            : this(catalogResolver, new CatalogIndexInfoResolver(catalogResolver), user)
		{
		}

        internal CatalogLiteralSanitizer(INameResolver<Catalog> catalogResolver, IIndexInfoResolver<Catalog> indexInfoResolver, User user)
		{
            if (catalogResolver == null)
            {
                throw new ArgumentNullException("catalogResolver");
            }

            if (indexInfoResolver == null)
            {
                throw new ArgumentNullException("indexInfoResolver");
            }

			_catalogResolver = catalogResolver;
			_catalogIndexInfoResolver = indexInfoResolver;
			this.user = user;
		}

		/// <summary>
		/// We make a big assumption here that a single project literal will never expand out into more than one project id,
		/// because of the rules around project names and resolving. This means that we should always get the same number of
		/// literals returned as that are passed in.
		/// </summary>
		/// <param name="literals"> the literals to sanitise; must not be null. </param>
		/// <returns> the result object containing the modification status and the resulting literals </returns>

        public LiteralSanitizerResult SanitiseLiterals(IEnumerable<QueryLiteral> literals)
        {
		    if (literals == null)
		    {
                throw new ArgumentNullException("literals");
		    }
            
            bool isModified = false;

            // keep a set of literals: if we're going to sanitise the literal, we may as well optimise and remove duplicates.
		    var resultantLiterals = new HashSet<QueryLiteral>();
            foreach (QueryLiteral literal in literals)
            {
                IEnumerable<string> stringIds = GetIndexValues(literal);
                foreach (string stringId in stringIds)
                {
                    var projectId = Convert.ToInt32(stringId);
                    Catalog project = _catalogResolver.Get(projectId);

                    // the only instance in which we sanitise is if the project existed, but the user does not have permission to see it.
                    if (project != null)
                    {
                        resultantLiterals.Add(new QueryLiteral(literal.SourceOperand, projectId));
                        isModified = true;
                    }
                    else
                    {
                        resultantLiterals.Add(literal);
                    }
                }
            }

            return new LiteralSanitizerResult(isModified, new List<QueryLiteral>(resultantLiterals));
        }

        internal virtual IEnumerable<string> GetIndexValues(QueryLiteral literal)
        {
            if (literal.StringValue != null)
            {
                return _catalogIndexInfoResolver.GetIndexedValues(literal.StringValue);
            }

            if (literal.IntValue != null)
            {
                return _catalogIndexInfoResolver.GetIndexedValues(literal.IntValue);
            }

            // must have got an Empty literal - but empty projects do not make sense, so just return an empty list.
            return new List<string>();
        }
    }
}