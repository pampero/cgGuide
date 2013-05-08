using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Helpers
{
    public static class HtmlHelperStaticContentExtensions
    {
        // http://stackoverflow.com/questions/3060775/stopping-cookies-being-set-from-a-domain-aka-cookieless-domain-to-increase-s/4843744#4843744
        public static string GetStaticContent(this UrlHelper url, string link)
        {
            link = link.ToLower();

            // En este caso funciona bien porque los archivos estan en el mismo filesystem. Habría que usar ETag. Aparte posiblemente tenga problemas de performance.
            var cacheBreaker = Convert.ToString(File.GetLastWriteTimeUtc(url.RequestContext.HttpContext.Request.MapPath(link)).Ticks, 16);

#if DEBUG
            return String.Format("~/content{0}?v={1}", link, cacheBreaker);
            
#else
            // Manejar desde el servidor estático lo referente a ETags. No sería necesario usar cacheBreaker.
            return String.Format("http://{0}{1}", ConfigurationManager.AppSettings["StaticContentServer"], link);

#endif

        }
    }
}