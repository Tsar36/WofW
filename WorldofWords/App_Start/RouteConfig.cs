
using System.Web.Mvc;
using System.Web.Routing;

namespace WorldofWords
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("elmah.axd");

            //Redirect to plain html and let Angular take care of the routing.
            routes.MapPageRoute(
            routeName: "AngularRoute",
            routeUrl: "Index",
            physicalFile: "~/Views/Index.html");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
