using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using SolrNet;

namespace Framework.Helpers
{
    public static class HtmlHelperMapperExtensions {
        private static IReadOnlyMappingManager mapper {
            get { return ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>(); }
        }

        //public static string SolrFieldPropName<T>(this HtmlHelper helper, string fieldName)
        //{
        //    return mapper.GetFields(typeof(T)).First(p => p.FieldName == fieldName).Property.Name;
        //}
    }
}