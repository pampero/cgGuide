using System.Configuration;
using Framework;
using Framework.Controllers;
using Framework.Filters;
using Framework.Solr.ViewModels;
using Framework.Solr.ViewModels.Binders;
using Frontend.Notifications;
using MvcGlobalisationSupport;
using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;
using ServiceStack.MiniProfiler;
using ServiceStack.Mvc.MiniProfiler;
using SolrNet;
using Model;
using Services.Routes.impl;
using Services.Routes.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebUI.Views.Config;
using log4net.Config;

namespace WebUI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
       protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ViewEngines.Engines.Insert(0, new LocalizedViewEngine());

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);           
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            LogManager.LogFactory = new Log4NetFactory(HttpContext.Current.Server.MapPath(@"\_config\log4net\log4net.xml"));
 
            ModelBinders.Binders[typeof(SearchParameters)] = new SearchParametersBinder();
//            Startup.Init<Product>("http://localhost:8983/solr");

        }


        /// <summary>
        /// INFO: Punto de salida para cualquier error no capturado en la aplicación.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = " ";
            var currentAction = " ";

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();

            var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Index";

            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;
                    default:
                        action = "Index";
                        break;
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;
            
            ILog logger = LogManager.GetLogger("LogFile");
            logger.Error(ex.Message, ex);

            controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }

        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                Profiler.Start();
            }
        }

        protected void Application_EndRequest()
        {
            Profiler.Stop();
        }
    }

}