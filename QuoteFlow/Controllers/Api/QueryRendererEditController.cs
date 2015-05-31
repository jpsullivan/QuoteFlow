using System.Web.Http;
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

        public string FieldId { get; set; }
        public string JqlContext { get; set; }

        // GET: api/QueryRendererEdit
        public IHttpActionResult Post()
        {
            var outcome = SearcherService.GetEditHtml(FieldId, JqlContext);
            if (outcome.IsValid())
            {
                return Ok(outcome.ReturnedValue);
            }

            var errors = outcome.ErrorCollection;
            return BadRequest();
        }
    }
}
