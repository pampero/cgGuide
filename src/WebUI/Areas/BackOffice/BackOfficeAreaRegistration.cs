using System.Web.Mvc;
using System.Web.Routing;
using MvcGlobalisationSupport;

namespace Framework.Areas.BackOffice
{
    public class BackOfficeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BackOffice";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BackOffice_Default2",
                "{culture}/BackOffice/{controller}/{action}/{id}",
                new
                {
                    culture = "en",
                    action = "Index",//ActionName
                    id = UrlParameter.Optional
                }
             ).RouteHandler = new LocalizedMvcRouteHandler();

            context.MapRoute(
                "BackOffice_default",
                "BackOffice/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}
