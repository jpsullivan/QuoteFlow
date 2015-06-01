using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using QuoteFlow.Api.Models.RequestModels.Api.Query;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Controllers.Api
{
    public class QueryRendererEditController : ApiController
    {
        #region DI

        public ISearcherService SearcherService { get; protected set; }

        public QueryRendererEditController(ISearcherService searcherService)
        {
            SearcherService = searcherService;
        }

        #endregion

        // GET: api/QueryRendererEdit
        public HttpResponseMessage Post([FromBody]QueryRendererEditPost model)
        {
            var outcome = SearcherService.GetEditHtml(model.FieldId, model.JqlContext);
            if (outcome.IsValid())
            {
                var response = new HttpResponseMessage();
                response.Content = new StringContent(outcome.ReturnedValue);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }

            var errors = outcome.ErrorCollection;
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }
    }
}
