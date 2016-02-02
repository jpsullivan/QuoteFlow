using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Context
{
    /// <summary>
    /// A context factory for asset SKU's and ID clauses. The catalog and manufacturer is taken 
    /// from the assets. If the operator is a negating operator then this returns a context with 
    /// <see cref="AllCatalogsContext"/> and <see cref="AllManufacturersContext"/>.
    /// </summary>
    public class AssetIdClauseContextFactory : IClauseContextFactory
    {
        private const int BatchMaxSize = 1000;

        private readonly IJqlAssetSupport _jqlAssetSupport;
        private readonly IJqlOperandResolver _jqlOperandResolver;
        private readonly ISet<Operator> _supportedOperators;

        public AssetIdClauseContextFactory(IJqlAssetSupport jqlAssetSupport, IJqlOperandResolver jqlOperandResolver, ISet<Operator> supportedOperators)
        {
            _jqlAssetSupport = jqlAssetSupport;
            _jqlOperandResolver = jqlOperandResolver;
            _supportedOperators = supportedOperators;
        }

        private ISet<KeyValuePair<int, int>> GetCatalogManufacturers(User searcher,
            IEnumerable<QueryLiteral> literals)
        {
            var catalogManufacturers = new HashSet<KeyValuePair<int, int>>();
            foreach (var literal in literals)
            {
                // if we have an empty literal, the Global context will not impact any existing contexts
                // so do nothing.
                if (!literal.IsEmpty)
                {
                    foreach (var asset in GetAssets(searcher, literal))
                    {
                        catalogManufacturers.Add(new KeyValuePair<int, int>(asset.CatalogId, asset.ManufacturerId));
                    }
                }
            }

            return catalogManufacturers;
        }

        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            throw new System.NotImplementedException();
        }

        private IEnumerable<IAsset> GetAssets(User searcher, QueryLiteral literal)
        {
            if (literal == null) throw new ArgumentNullException(nameof(literal));

            IAsset asset;
            if (literal.IntValue != null)
            {
                asset = _jqlAssetSupport.GetAsset((int) literal.IntValue, searcher);
            }
            else
            {
                throw new InvalidOperationException("Invalid query literal.");
            }

            return asset != null ? new List<IAsset> {asset} : new List<IAsset>();
        }
    }
}