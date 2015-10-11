using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Controllers.Api
{
    public class QuoteStatusController : ApiController
    {
        #region DI

        protected IQuoteStatusService QuoteStatusService { get; }

        public QuoteStatusController() { }

        public QuoteStatusController(IQuoteStatusService quoteStatusService)
        {
            QuoteStatusService = quoteStatusService;
        }

        #endregion

        public IEnumerable<QuoteStatus> Get()
        {
            return QuoteStatusService.GetStatuses(1);
        }

        /// <summary>
        /// Fetches a collection of <see cref="QuoteStatus"/> objects based on
        /// their assigned organization.
        /// </summary>
        /// <param name="id">The organization id to fetch quote statuses from.</param>
        /// <returns></returns>
        public IEnumerable<QuoteStatus> Get(int id)
        {
            if (id == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return QuoteStatusService.GetStatuses(id);
        }

        public void Post(QuoteStatus status)
        {
            if (status == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            QuoteStatusService.CreateStatus(status);
        }

        public void Put(QuoteStatus status)
        {
            if (status == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            //QuoteStatusService.InsertAssetVar(assetVar);
        }

        public void Delete(int id)
        {
            if (id == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            QuoteStatusService.DeleteStatus(id);
        }
    }
}
