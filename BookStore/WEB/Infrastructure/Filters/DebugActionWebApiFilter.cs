using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WEB.Infrastructure.Filters
{
    public class DebugActionWebApiFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // pre-processing
            Debug.WriteLine("DEBUG pre-processing logging");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var objectContent = actionExecutedContext.Response.Content as ObjectContent;
            if (objectContent != null)
            {
                var type = objectContent.ObjectType; //type of the returned object
                var value = objectContent.Value; //holding the returned value
            }

            Debug.WriteLine("DEBUG OnActionExecuted Response " + actionExecutedContext.Response.StatusCode.ToString());
        }
    }
}