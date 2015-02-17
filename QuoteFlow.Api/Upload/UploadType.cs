namespace QuoteFlow.Api.Upload
{
    /// <summary>
    /// The different types of files that can be uploaded throughout QuoteFlow.
    /// </summary>
    public enum UploadType
    {
        AssetImage,
        Catalog,
        ManufacturerLogo
    }

    public static class UploadTypeExtensions
    {
        /// <summary>
        /// Gets the upload folder name based on the specified upload type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetUploadFolder(this UploadType type)
        {
            switch (type)
            {
                case UploadType.AssetImage:
                    return "asset-images";
                case UploadType.Catalog:
                    return "catalogs";
                case UploadType.ManufacturerLogo:
                    return "manufacturer-logos";
                default:
                    return Constants.UploadsFolderName;
            }
        }
    }
}
