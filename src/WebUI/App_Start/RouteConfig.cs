using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcGlobalisationSupport;

namespace Framework
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            const string defautlRouteUrl = "{controller}/{action}/{id}";
            RouteValueDictionary defaultRouteValueDictionary = new RouteValueDictionary(new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("api/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add("DefaultGlobalized", new GlobalisedRoute(defautlRouteUrl, defaultRouteValueDictionary));
            routes.Add("Default", new System.Web.Routing.Route(defautlRouteUrl, defaultRouteValueDictionary, new MvcRouteHandler()));

            //LocalizedViewEngine: this is to support views also when named e.g. Index.es-AR.cshtml
            routes.MapRoute(
                "DefaultLocalized",
                "{culture}/{controller}/{action}/{id}",
                new
                {
                    culture = "en",
                    controller = "Home",//ControllerName
                    action = "Index",//ActionName
                    id = UrlParameter.Optional
                }
            ).RouteHandler = new LocalizedMvcRouteHandler();

            routes.MapRoute("CatchAll", "{*url}",
                new { controller = "Home", action = "Index" }
            );
        }
    }
}