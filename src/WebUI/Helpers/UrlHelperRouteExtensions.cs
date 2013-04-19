using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Framework.Helpers
{
    /// <summary>
    /// Route parameters processing extensions
    /// </summary>
    public static class UrlHelperRouteExtensions {
        /// <summary>
        /// Sets/changes the current route parameters
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="parameters">Parameters to set/change</param>
        /// <returns>Resulting URL</returns>
        public static string SetRouteParameters(this UrlHelper helper, IDictionary<string, object> parameters) {
            var routeParams = new RouteValueDictionary(helper.RequestContext.RouteData.Values);
            foreach (var p in parameters)
                routeParams[p.Key] = p.Value;
            return helper.RouteUrl(routeParams);
        }

        /// <summary>
        /// Sets/changes the current route parameters
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="parameters"></param>
        /// <returns>Resulting URL</returns>
        public static string SetRouteParameters(this UrlHelper helper, object parameters) {
            return helper.SetRouteParameters(parameters.ToPropertyDictionary());
        }

        /// <summary>
        /// Sets/changes a single route parameter
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="key">Route param key</param>
        /// <param name="value">Route param value</param>
        /// <returns>Resulting URL</returns>
        public static string SetRouteParameter(this UrlHelper helper, string key, object value) {
            return helper.SetRouteParameters(new Dictionary<string, object> {
                {key, value},
            });
        }
    }
}