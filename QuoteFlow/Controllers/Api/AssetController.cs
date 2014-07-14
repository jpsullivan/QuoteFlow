using System.Net;
using System.Web.Http;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Controllers.Api
{
    public class AssetController : ApiController
    {
        #region IoC

        protected IAssetService AssetService { get; set; }

        public AssetController() { }

        public AssetController(IAssetService assetService)
        {
            AssetService = assetService;
        }

        #endregion

        public Asset Get(int id)
        {
            if (id == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return AssetService.GetAsset(id);
        }
    }
}