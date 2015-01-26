namespace QuoteFlow.Api.Models
{
    public class OptionalImportField
    {
        public OptionalImportField() { }

        public OptionalImportField(int assetVarId, int headerId)
        {
            AssetVarId = assetVarId;
            HeaderId = headerId;
        }

        /// <summary>
        /// The id of the <see cref="AssetVar"/> that the header
        /// will be assigned to.
        /// </summary>
        public int AssetVarId { get; set; }

        /// <summary>
        /// The catalog header id that is going to be used
        /// in conjunction with a specified <see cref="AssetVarId"/>
        /// </summary>
        public int HeaderId { get; set; }
    }
}