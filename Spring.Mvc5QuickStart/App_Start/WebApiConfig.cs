using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Spring.Mvc5QuickStart.Filters;

namespace Spring.Mvc5QuickStart
{

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new CustomExceptionFilterAttribute());
            
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("controller", "api/{controller}/{action}", new { action = RouteParameter.Optional  });
        }
    }
}