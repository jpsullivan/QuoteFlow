using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Resolver;

namespace QuoteFlow.Models.Assets.Search.Searchers.Transformer
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