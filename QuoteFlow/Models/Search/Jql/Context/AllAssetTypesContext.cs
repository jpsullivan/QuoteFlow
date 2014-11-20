namespace QuoteFlow.Models.Search.Jql.Context
{
    public class AllAssetTypesContext : IAssetTypeContext
    {
        private static readonly AllAssetTypesContext INSTANCE = new AllAssetTypesContext();

		public static AllAssetTypesContext Instance
		{
			get { return INSTANCE; }
		}

        private AllAssetTypesContext()
		{
			//Don't create me.
		}

        public string AssetTypeId { get { return null; } }
        
        public bool All { get { return true; } }

        public override string ToString()
        {
            return "AllAsset Types Context";
        }
    }
}