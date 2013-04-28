using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
 
// http://www.pbworks.net/multilanguage-mvc4-website/
public class LocalizedMvcRouteHandler : MvcRouteHandler
{
	protected override IHttpHandler GetHttpHandler (RequestContext requestContext)
	{
       CultureInfo ci = new CultureInfo(requestContext.RouteData.Values["culture"].ToString());
 
		Thread.CurrentThread.CurrentUICulture = ci;
		Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
 
		return base.GetHttpHandler(requestContext);
	}
}