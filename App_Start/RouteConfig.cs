using System.Web.Mvc;
using System.Web.Routing;

namespace _20104681JoshMkhariCLDV6212Task2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                //set default view
                defaults: new { controller = "Product", action = "Get", id = UrlParameter.Optional }
            );
        }
    }
}
