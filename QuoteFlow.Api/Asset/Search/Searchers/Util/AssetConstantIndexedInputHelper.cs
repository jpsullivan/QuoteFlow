using System;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Resolver;

namespace QuoteFlow.Api.Asset.Search.Searchers.Util
{
    /// <summary>
    /// Extension of <see cref="IndexedInputHelper{T}"/> that knows how to create 
    /// <see cref="SingleValueOperand"/>s by resolving ids to Issue Constant names.
    /// </summary>
    public class AssetConstantIndexedInputHelper<T> : IndexedInputHelper<T> where T : IAssetConstant
    {
        private readonly INameResolver<T> assetConstantResolver;

        public AssetConstantIndexedInputHelper(IIndexInfoResolver<T> indexInfoResolver, IJqlOperandResolver operandResolver, IFieldFlagOperandRegistry fieldFlagOperandRegistry, INameResolver<T> assetConstantResolver)
            : base(indexInfoResolver, operandResolver, fieldFlagOperandRegistry)
        {
            if (assetConstantResolver == null)
            {
                throw new ArgumentNullException(nameof(assetConstantResolver));
            }

            this.assetConstantResolver = assetConstantResolver;
        }

        protected override SingleValueOperand CreateSingleValueOperandFromId(string stringValue)
        {
            int issueConstantId;
            try
            {
                issueConstantId = Convert.ToInt32(stringValue);
            }
            catch (Exception e)
            {
                return new SingleValueOperand(stringValue);
            }

            IAssetConstant issueConstant = assetConstantResolver.Get(issueConstantId);

            if (issueConstant != null)
            {
                return new SingleValueOperand(issueConstant.Name);
            }
            return new SingleValueOperand(issueConstantId);
        }
    }
}