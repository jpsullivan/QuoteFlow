using System;
using QuoteFlow.Api.Asset.Search.Searchers.Util;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Resolver;

namespace QuoteFlow.Api.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// An assset-constant-specific <see cref="IdIndexedSearchInputTransformer"/>.
    /// </summary>
    public class AssetConstantSearchInputTransformer<T> : IdIndexedSearchInputTransformer<T> where T : IAssetConstant
    {
        private readonly INameResolver<T> assetConstantResolver;

        public AssetConstantSearchInputTransformer(ClauseNames clauseNames, IIndexInfoResolver<T> indexInfoResolver, IJqlOperandResolver operandResolver, IFieldFlagOperandRegistry fieldFlagOperandRegistry, INameResolver<T> assetConstantResolver)
            : base(clauseNames, indexInfoResolver, operandResolver, fieldFlagOperandRegistry)
        {
            if (assetConstantResolver == null)
            {
                throw new ArgumentNullException("assetConstantResolver");
            }

            this.assetConstantResolver = assetConstantResolver;
        }

        public AssetConstantSearchInputTransformer(ClauseNames jqlClauseNames, string urlParameter, IIndexInfoResolver<T> indexInfoResolver, IJqlOperandResolver operandResolver, IFieldFlagOperandRegistry fieldFlagOperandRegistry, INameResolver<T> assetConstantResolver)
            : base(jqlClauseNames, urlParameter, indexInfoResolver, operandResolver, fieldFlagOperandRegistry)
        {
            if (assetConstantResolver == null)
            {
                throw new ArgumentNullException("assetConstantResolver");
            }

            this.assetConstantResolver = assetConstantResolver;

        }

        internal override IIndexedInputHelper createIndexedInputHelper()
        {
            return new AssetConstantIndexedInputHelper<T>(indexInfoResolver, operandResolver, fieldFlagOperandRegistry, assetConstantResolver);
        }
    }
}