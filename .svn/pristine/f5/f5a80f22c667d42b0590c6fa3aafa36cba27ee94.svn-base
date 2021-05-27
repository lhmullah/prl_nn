using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PayrollWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //routes.MapRoute(
            //    name: "test",
            //    url: "Deduction/GetEmployeeSeach/query",
            //    defaults: new { controller = "Deduction", action = "GetEmployeeSeach", query = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "LogOn",
                url: "{controller}/{action}/{dnid}",
                defaults: new { controller = "Authentication", action = "LogOn", dnid = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            // Add our route registration for MvcSiteMapProvider sitemaps
            MvcSiteMapProvider.Web.Mvc.XmlSiteMapController.RegisterRoutes(routes);
        }
    }
}