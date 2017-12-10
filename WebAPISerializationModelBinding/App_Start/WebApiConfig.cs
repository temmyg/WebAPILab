using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Spring.Mvc5QuickStart.Filters;
using System.Net.Http.Formatting;

namespace Spring.Mvc5QuickStart
{

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = 
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling =
                Newtonsoft.Json.PreserveReferencesHandling.All;

            // remove serialized Json object has $id property
            //JsonMediaTypeFormatter jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().Single();
            //jsonFormatter.UseDataContractJsonSerializer = false;
            //jsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //jsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //jsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            //config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

            config.Filters.Add(new CustomExceptionFilterAttribute());

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("controller", "api/{controller}/{action}", new { action = RouteParameter.Optional });
        }
    }
}