using System.Web.Mvc;

namespace WebUI.Areas.Solr
{
    public class POCAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "POC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "POC_default",
                "POC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
