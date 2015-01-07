using System.Web.Http;

namespace QuoteFlow.Controllers.Admin
{
    [Authorize(Roles = Constants.AdminRoleName)]
    public class AdminControllerBase : AppController
    {
    }
}