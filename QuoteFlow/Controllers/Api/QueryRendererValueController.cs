using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Services;
using Wintellect.PowerCollections;

namespace QuoteFlow.Controllers.Api
{
    public class QueryRendererValueController : ApiController
    {
        #region DI

        public IUserService UserService { get; protected set; }
        public ISearcherService SearcherService { get; protected set; }

        public QueryRendererValueController(IUserService userService, ISearcherService searcherService)
        {
            UserService = userService;
            SearcherService = searcherService;
        }

        #endregion

        // GET: api/QueryRendererEdit
        public HttpResponseMessage Post()
        {
            // since post args can contain duplicate keys, we cannot rely on model binding
            var data = Request.Content.ReadAsStringAsync().Result;
            if (data.IsNullOrWhiteSpace())
            {
                return null;
            }

            var dataArray = data.Split('&');
            var multiDict = new MultiDictionary<string, string[]>(true);
            foreach (var arg in dataArray)
            {
                var args = arg.Split('=');
                multiDict.Add(args[0], new[] { HttpUtility.UrlDecode(args[1]) });
            }

            // also get the user
            var user = UserService.GetUser(RequestContext.Principal.Identity.Name, null);

            var outcome = SearcherService.GetViewHtml(user, multiDict);
            if (outcome.IsValid())
            {
                var response = new HttpResponseMessage();
                response.Content = new ObjectContent(typeof(SearchRendererValueResults), outcome.ReturnedValue, new JsonMediaTypeFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }

            var errors = outcome.ErrorCollection;
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }
    }
}
