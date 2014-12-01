using System;
using QuoteFlow.Models.Assets.Search.Searchers.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Resolver;

namespace QuoteFlow.Models.Assets.Search.Searchers.Transformer
{
    /// <summary>
    /// An assset-constant-specific <seealso cref="IdIndexedSearchInputTransformer"/>.
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