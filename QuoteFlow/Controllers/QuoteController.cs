using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Services.Interfaces;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public class QuoteController : BaseController
    {
        #region IoC

        public IQuoteService QuoteService { get; protected set; }

        public QuoteController() { }

        public QuoteController(IQuoteService quoteService)
        {
            QuoteService = quoteService;
        }

        #endregion

        [Route("quotes")]
        public ActionResult Index()
        {
            var quotes = QuoteService.GetQuotesFromOrganization(CurrentOrganization.Id);

            return View(quotes);
        }

        [Route("quote/new")]
        [LayoutInjector("_LayoutQuoteWorkflow")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("quote/create", HttpVerbs.Post)]
        public ActionResult CreateQuote(NewQuoteModel quoteForm)
        {
            if (!ModelState.IsValid) return New();

            // check to make sure that a quote with this name doesn't already exist
            if (QuoteService.ExistsWithinOrganization(quoteForm.Name, CurrentOrganization.Id))
            {

            }

            return new EmptyResult();
        }
    }
}
