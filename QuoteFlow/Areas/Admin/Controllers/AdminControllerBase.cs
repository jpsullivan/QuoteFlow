using System.Web.Mvc;
using QuoteFlow.Controllers;

namespace QuoteFlow.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.AdminRoleName)]
    public class AdminControllerBase : AppController
    {
    }
}