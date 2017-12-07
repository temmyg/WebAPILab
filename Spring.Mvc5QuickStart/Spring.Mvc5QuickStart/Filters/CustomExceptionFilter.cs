using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Spring.Mvc5QuickStart.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response = new HttpResponseMessage()
            {
                Content = new StringContent("not intended invocation"),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                actionExecutedContext.Response = new HttpResponseMessage()
                {
                    Content = new StringContent("not intended invocation"),
                    StatusCode = HttpStatusCode.BadRequest
                };
            });
        }
    }
}