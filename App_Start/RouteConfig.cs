using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Exersice3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Default", "{controller}/{action}",
               defaults: new { controller = "Home", action = "index" });

            routes.MapRoute("display","display/{ip}/{port}",
                defaults: new { controller = "Home", action = "display"}
            );
            routes.MapRoute("Refresh", "display/{ip}/{port}/{timeSlice}",
               defaults: new {controller = "Home", action = "Refresh" } 
               );
            routes.MapRoute("save", "display/{ip}/{port}/{timeSlice}/{timePeriod}/{file}",
                defaults: new {controller = "Home", action = "save"}
                );
           
        }
    }
}
