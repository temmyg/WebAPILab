using System.Web;
using System.Web.Mvc;
using Spring.Mvc5QuickStart.Filters;

namespace Spring.Mvc5QuickStart
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // filters.Add(new CustomExceptionFilterAttribute());
        }
    }
}