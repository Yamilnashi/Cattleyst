using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CattleystWebPortal.Filters
{
    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.HttpContext.Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                context.Result = new StatusCodeResult(403); // forbidden
            }
            base.OnActionExecuted(context);
        }
    }
}
