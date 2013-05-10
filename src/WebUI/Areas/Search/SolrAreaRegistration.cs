using System.Web.Mvc;

namespace WebUI.Areas.Solr
{
    public class SearchAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Search";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Search_default",
                "Search/{action}/{id}",
                new { controller = "Solr", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
