using QuoteFlow.Models.ViewModels;

namespace QuoteFlow.Services.Interfaces
{
    public interface ICatalogImportService
    {
        /// <summary>
        /// Performs the import operations for a catalog.
        /// </summary>
        /// <param name="model">The ViewModel'd container for the import fields and rules.</param>
        /// <param name="currentUserId"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        int ImportCatalog(VerifyCatalogImportViewModel model, int currentUserId, int organizationId);
    }
}