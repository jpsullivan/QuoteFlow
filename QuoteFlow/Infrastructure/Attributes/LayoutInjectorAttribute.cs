using System.Web.Mvc;

namespace QuoteFlow.Infrastructure.Attributes
{
    public class LayoutInjectorAttribute : ActionFilterAttribute
    {
        private readonly string _masterName;
        public LayoutInjectorAttribute(string masterName)
        {
            _masterName = masterName;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var result = filterContext.Result as ViewResult;
            if (result != null)
            {
                result.MasterName = _masterName;
            }
        }
    }
}