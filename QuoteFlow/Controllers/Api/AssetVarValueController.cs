using System.Net;
using System.Web.Http;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Controllers.Api
{
    public class AssetVarValueController : ApiController
    {
        #region IoC

        protected IAssetVarService AssetVarService { get; set; }

        public AssetVarValueController() { }

        public AssetVarValueController(IAssetVarService assetVarService)
        {
            AssetVarService = assetVarService;
        }

        #endregion

        public void Post(AssetVarValue varValue)
        {
            if (varValue == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            AssetVarService.InsertVarValue(varValue);
        }

        public void Put(AssetVarValue varValue)
        {
            if (varValue == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            AssetVarService.InsertVarValue(varValue);
        }

        public void Delete(int id)
        {
            if (id == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            AssetVarService.DeleteAssetVarValue(id);
        }
    }
}
