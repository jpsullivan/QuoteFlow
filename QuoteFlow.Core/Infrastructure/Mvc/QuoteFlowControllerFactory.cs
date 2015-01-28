using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuoteFlow.Core.Infrastructure.Mvc
{
    public class QuoteFlowControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var controller = base.GetControllerInstance(requestContext, controllerType);
            //var controllerBase = controller as Controller;
            //controllerBase.TempDataProvider = new CookieTempDataProvider(requestContext.HttpContext);
            return controller;
        }
    }
}