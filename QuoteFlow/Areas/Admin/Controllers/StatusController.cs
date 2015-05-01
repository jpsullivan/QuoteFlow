using System.Web.Mvc;
using QuoteFlow.Api.Services;
using QuoteFlow.Areas.Admin.ViewModels;
using QuoteFlow.Infrastructure.Attributes;

namespace QuoteFlow.Areas.Admin.Controllers
{
    public class StatusController : AdminControllerBase
    {
        #region DI

        public IQuoteStatusService QuoteStatusService { get; protected set; }

        public StatusController(IQuoteStatusService quoteStatusService)
        {
            QuoteStatusService = quoteStatusService;
        }

        #endregion

        [QuoteFlowRoute("admin/statuses", Name = "Admin-Statuses")]
        public ActionResult Index()
        {
            var statuses = QuoteStatusService.GetStatuses(1); // todo remove organization hardcode
            var model = new QuoteStatusListModel(statuses);
            return View(model);
        }
    }
}