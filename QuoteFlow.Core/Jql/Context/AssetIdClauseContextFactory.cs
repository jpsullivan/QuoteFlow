using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
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

        private ISet<KeyValuePair<int, string>> GetCatalogManufacturersBatch(User searcher, IEnumerable<QueryLiteral> literals)
        {
            var numericLiterals = new HashSet<int>();
            var stringLiterals = new HashSet<string>();
            foreach (var literal in literals.Where(literal => !literal.IsEmpty))
            {
                if (literal.IntValue != null)
                {
                    numericLiterals.Add(literal.IntValue.Value);
                }
                else if (literal.StringValue != null)
                {
                    stringLiterals.Add(literal.StringValue);
                }
            }

            var catalogManufacturers = new HashSet<KeyValuePair<int, string>>();
            if (numericLiterals.Any())
            {
                foreach (var pair in _jqlAssetSupport.GetCatalogManufacturerPairsByIds(numericLiterals))
                {
                    catalogManufacturers.Add(pair);
                }
            }

            if (stringLiterals.Any())
            {
                foreach (var pair in _jqlAssetSupport.GetCatalogManufacturerPairsBySkus(stringLiterals))
                {
                    catalogManufacturers.Add(pair);
                }
            }

            return catalogManufacturers;
        }

        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            var @operator = terminalClause.Operator;
            if (!HandlesOperator(@operator) || IsNegationOperator(@operator) || IsEmptyOperator(@operator))
            {
                return ClauseContext.CreateGlobalClauseContext();
            }

            var literals = _jqlOperandResolver.GetValues(searcher, terminalClause.Operand, terminalClause).ToList();
            if (!literals.Any())
            {
                return ClauseContext.CreateGlobalClauseContext();
            }

            var contexts = new HashSet<ICatalogManufacturerContext>();
            int batches = literals.Count/BatchMaxSize + 1;
            for (int batchIndex = 0; batchIndex < batches; batchIndex++)
            {
                var literalsBatch = literals.GetRange(batchIndex*BatchMaxSize, Math.Min((batchIndex + 1)*BatchMaxSize, literals.Count));
                var catalogManufacturers = GetCatalogManufacturersBatch(searcher, literalsBatch);
                foreach (var catalogManufacturer in catalogManufacturers)
                {
                    if (IsEqualsOperator(@operator))
                    {
                        contexts.Add(new CatalogManufacturerContext(new CatalogContext(catalogManufacturer.Key),
                            new ManufacturerContext(Convert.ToInt32(catalogManufacturer.Value))));
                    }
                    else if (IsRelationalOperator(@operator))
                    {
                        contexts.Add(new CatalogManufacturerContext(new CatalogContext(catalogManufacturer.Key),
                            new ManufacturerContext(Convert.ToInt32(catalogManufacturer.Value))));
                    }
                }
            }

            return contexts.Any() ? new ClauseContext(contexts) : ClauseContext.CreateGlobalClauseContext();
        }

        private bool IsEmptyOperator(Operator @operator)
        {
            return OperatorClasses.EmptyOnlyOperators.Contains(@operator);
        }

        private bool IsNegationOperator(Operator @operator)
        {
            return OperatorClasses.NegativeEqualityOperators.Contains(@operator);
        }

        private bool IsRelationalOperator(Operator @operator)
        {
            return OperatorClasses.RelationalOnlyOperators.Contains(@operator);
        }

        private bool IsEqualsOperator(Operator @operator)
        {
            return @operator == Operator.EQUALS || @operator == Operator.IN;
        }

        private bool HandlesOperator(Operator @operator)
        {
            return _supportedOperators.Contains(@operator);
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

        public class Factory
        {
            private readonly IJqlAssetSupport _assetSupport;
            private readonly IJqlOperandResolver _operandResolver;

            public Factory(IJqlAssetSupport assetSupport, IJqlOperandResolver operandResolver)
            {
                _assetSupport = assetSupport;
                _operandResolver = operandResolver;
            }

            public virtual AssetIdClauseContextFactory Create(ISet<Operator> supportedOperators)
            {
                return new AssetIdClauseContextFactory(_assetSupport, _operandResolver, supportedOperators);
            }
        }
    }
}