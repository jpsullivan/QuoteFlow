using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// The search input transformer for the manufacturer system field.
    /// </summary>
    public class ManufacturerSearchInputTransformer : AssetConstantSearchInputTransformer<Manufacturer>
    {
        public ManufacturerSearchInputTransformer(IIndexInfoResolver<Manufacturer> indexInfoResolver, 
            IJqlOperandResolver operandResolver, IFieldFlagOperandRegistry fieldFlagOperandRegistry, 
            INameResolver<Manufacturer> nameResolver)
            : base(SystemSearchConstants.ForManufacturer().JqlClauseNames, SystemSearchConstants.ForManufacturer().UrlParameter, indexInfoResolver, operandResolver, fieldFlagOperandRegistry, nameResolver)
        {
		}
    }
}