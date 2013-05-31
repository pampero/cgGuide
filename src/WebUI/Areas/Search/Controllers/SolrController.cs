using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;
using Common.Base;
using Framework.Solr.ViewModels;
using Microsoft.Practices.ServiceLocation;
using ServiceStack.Text;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.DSL;
using SolrNet.Exceptions;
using SearchType = Model.Enums.SearchType;

namespace WebUI.Areas.Solr.Controllers
{
    // http://wiki.apache.org/solr/CommonQueryParameters#fq
    [Authorize]
    public partial class SolrController : BaseController
    {
        private readonly ISolrOperations<SolrBusiness> solr;
        private readonly string[] AllFacetFields = new[] { "paths", "attributes", "rating" };

        public SolrController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<SolrBusiness>>();
        }

        public class CheckedItemDto
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public bool Checked { get; set; }
        }

        // El argumento parameters almacena los parametros de búsqueda en la vista, tanto el freesearch como las facetas seleccionadas
        // SE ENCARGA SOLO DEL ARMADO DEL FACETADO
        public ActionResult Index(SearchParameters parameters)
        {
            try
            {
                var start = (parameters.PageIndex - 1) * parameters.PageSize;

                var queryOptions = new QueryOptions
                {
                    FilterQueries = BuildFilterQuery(parameters, SearchType.FacetSearch),
                    Rows = parameters.PageSize,
                    Start = start,
                    OrderBy = GetSelectedSort(parameters),
                    SpellCheck = new SpellCheckingParameters(),
                    Facet = BuildFacetParameters(parameters),
                };


                var matchingSellers = solr.Query(BuildQuery(parameters), queryOptions);

                var view = new SolrBussinesViewModel
                {
                    Sellers = matchingSellers,
                    Search = parameters,
                    TotalCount = matchingSellers.NumFound,
                    Facets = matchingSellers.FacetFields
                };

                return View(view);
            }
            catch (InvalidFieldException)
            {
                return View(new SolrBussinesViewModel
                {
                    QueryError = true,
                });
            }
        }

        private SearchParameters GetSearchParametersFromCookie()
        {
            SearchParameters parameters = null;

            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("SearchParametersCookie"))
            {
                var cookie = ControllerContext.HttpContext.Request.Cookies["SearchParametersCookie"];
                parameters = cookie.Value.FromJson<SearchParameters>();
            }

            return parameters;
        }

        // LISTA LOS SELLERS DE ACUERDO A LA BUSQUEDA INICIAL Y A LA SELECCION DE LOS CHECKBOX 'checkedItemDtoItemDto'
        public ActionResult GetAllBussines(CheckedItemDto checkedItemDto)
        {
            var parameters = GetSearchParametersFromCookie();

            UpdateBreadCrumbForFilterQuery(checkedItemDto, parameters);

            WriteCookie(parameters);

            // UTILIZA FILTER QUERY PARA FILTRAR SOBRE LO YA SELECCIONADO EN LAS FACETAS -ACTION INDEX-
            var sellers = solr.Query(BuildQuery(parameters), new QueryOptions
                            {
                                FilterQueries = BuildFilterQuery(parameters, SearchType.ListSearch),
                                Rows = 5,
                                Start = 0,
                                SpellCheck = new SpellCheckingParameters()
                            });

            return Json(new { sellers }, JsonRequestBehavior.AllowGet);
        }

        private void UpdateBreadCrumbForFilterQuery(CheckedItemDto checkedItemDto, SearchParameters parameters)
        {
            if (checkedItemDto == null) return;

            var ckeckedItem = parameters.BreadCrumb.FirstOrDefault(x => x.Key == checkedItemDto.Key.ToLower());

            if (ckeckedItem.Key == null)
            {
                if (checkedItemDto.Checked)
                {
                    parameters.BreadCrumb.Add(new KeyValuePair<string, string>(checkedItemDto.Key.ToLower(),
                                                                               checkedItemDto.Value));
                }
            }
            else
            {
                if (checkedItemDto.Checked)
                {   // AGREGO
                    if (!parameters.BreadCrumb[ckeckedItem.Key].Contains(checkedItemDto.Value))
                        parameters.BreadCrumb[ckeckedItem.Key] = parameters.BreadCrumb[ckeckedItem.Key] + "|" + checkedItemDto.Value;
                }
                else
                {   // ELIMINO
                    if (parameters.BreadCrumb[ckeckedItem.Key].Contains(checkedItemDto.Value))
                    {
                        parameters.BreadCrumb[ckeckedItem.Key] =
                            parameters.BreadCrumb[ckeckedItem.Key].Replace("|" + checkedItemDto.Value, "").Replace(
                                checkedItemDto.Value + "|", "").Replace(checkedItemDto.Value, "");

                        if (parameters.BreadCrumb[ckeckedItem.Key] == "")
                            parameters.BreadCrumb.Remove(ckeckedItem.Key);
                    }
                }
            }
        }


        // BUSQUEDA LIBRE
        public ISolrQuery BuildQuery(SearchParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.FreeSearch))
                return new SolrQuery(parameters.FreeSearch + "*");
            return SolrQuery.All;
        }


        public SortOrder[] GetSelectedSort(SearchParameters parameters)
        {
            return new[] { SortOrder.Parse(parameters.Sort) }.Where(o => o != null).ToArray();
        }

        // BUSQUEDA PARA FACETADO(FACET)/LISTADO(BREADCRUMB) + FILTRO CIUDAD
        private ICollection<ISolrQuery> BuildFilterQuery(SearchParameters parameters, Model.Enums.SearchType searchType)
        {
            var listFilterQuery = new List<ISolrQuery>();

            if (searchType == Model.Enums.SearchType.FacetSearch)
            {
                listFilterQuery = parameters.Facets.Where(x => x.Value.Contains("|")).Select(p => (ISolrQuery)Query.Field(p.Key).In(p.Value.Split('|'))).ToList();
            }
            else
            {
                foreach (var key in parameters.BreadCrumb.Keys)
                {
                    var values = parameters.BreadCrumb[key.ToLower()].Split('|');

                    foreach (var value in values)
                    {
                        var fq = (ISolrQuery)Query.Field(key.ToLower()).Is(value);
                        listFilterQuery.Add(fq);
                    }
                }
            }

            return AddCitySearch(parameters, listFilterQuery);
        }
        

        private static ICollection<ISolrQuery> AddCitySearch(SearchParameters parameters, ICollection<ISolrQuery> listFilterQuery)
        {
            if (!string.IsNullOrEmpty(parameters.SearchCity))
            {
                var fq = (ISolrQuery) Query.Field("city").Is(parameters.SearchCity + "*");
                listFilterQuery.Add(fq);
            }

            return listFilterQuery;
        }
        


        private void WriteCookie(SearchParameters parameters)
        {
            var searchParametersCookie = new HttpCookie("SearchParametersCookie") {Value = parameters.ToJson()};
            ControllerContext.HttpContext.Response.Cookies.Add(searchParametersCookie);
        }

        private FacetParameters BuildFacetParameters(SearchParameters parameters)
        {
            var facetParameters = new FacetParameters();

            foreach (var data in AllFacetFields)
            {
                ISolrFacetQuery facetQuery;

                if (data == "paths")
                {
                    var prefix = string.Empty;
                    var facet = parameters.Facets.SingleOrDefault(x => x.Key == "paths");

                    foreach (var facetAux in parameters.Facets)
                        parameters.BreadCrumb.Add(new KeyValuePair<string, string>(facetAux.Key, facetAux.Value));

                    parameters.Facets.Clear();

                    if (facet.Key != null)
                    {
                        var depth = facet.Value.Split('/').Count() - 1 + "/";

                        prefix = facet.Value.Replace(facet.Value.Substring(0, facet.Value.IndexOf('/') + 1), depth);
                    }
                    else
                        prefix = "0/";

                    facetQuery = new SolrFacetFieldQuery("paths") {MinCount = 1, Prefix = prefix};
                }
                else
                    facetQuery = new SolrFacetFieldQuery(data) {MinCount = 1};

                facetParameters.Queries.Add(facetQuery);
            }

            WriteCookie(parameters);

            return facetParameters;
        }
    }
}
