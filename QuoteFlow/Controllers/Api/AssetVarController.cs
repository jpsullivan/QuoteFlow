using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Controllers.Api
{
    public class AssetVarController : ApiController
    {
        #region IoC

        protected IAssetVarService AssetVarService { get; set; }

        public AssetVarController() { }

        public AssetVarController(IAssetVarService assetVarService)
        {
            AssetVarService = assetVarService;
        }

        #endregion

        /// <summary>
        /// Fetches a collection of <see cref="AssetVar"/> objects based on
        /// their assigned organization.
        /// </summary>
        /// <param name="id">The organization id to fetch asset vars from.</param>
        /// <returns></returns>
        public IEnumerable<AssetVar> Get(int id)
        {
            if (id == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return AssetVarService.GetAssetVarsByOrganizationId(id);
        }

        public void Post(AssetVar assetVar)
        {
            if(assetVar == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            AssetVarService.InsertAssetVar(assetVar);
        }

        public void Put(AssetVar assetVar)
        {
            if (assetVar == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            AssetVarService.InsertAssetVar(assetVar);
        }

        public void Delete(int id)
        {
            if (id == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            AssetVarService.DeleteAssetVar(id);
        }
    }
}
