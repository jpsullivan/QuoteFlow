using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields.Option;

namespace QuoteFlow.Core.Asset.Fields.Option
{
    public class AssetConstantOption : AbstractOption, IOption
    {
        private IAssetConstant _constant;

        public AssetConstantOption(IAssetConstant constant)
        {
            _constant = constant;
        }

        public string Id { get { return _constant.Id.ToString(); } }
        public string Name { get { return _constant.Name; } }
        public string Description { get { return _constant.Description; } }
    }
}