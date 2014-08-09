using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace QuoteFlow.Controllers.Admin
{
    [Authorize(Roles = Constants.AdminRoleName)]
    public class AdminControllerBase : AppController
    {
    }
}