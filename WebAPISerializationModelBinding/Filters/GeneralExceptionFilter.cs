using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;

namespace Spring.Mvc5QuickStart.Filters
{
    public class GeneralExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Format("exception thrown in action method with {0} \r\n {1}", actionExecutedContext.Exception.Message,
                                actionExecutedContext.Exception.StackTrace))
            };
            base.OnException(actionExecutedContext);
        }
    }
}