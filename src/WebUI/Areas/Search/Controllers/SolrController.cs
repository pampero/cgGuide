using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Common.Base;
using Framework.Helpers;
using Framework.Solr.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model;
using ServiceStack.Text;
using Services.Routes.interfaces;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.DSL;
using SolrNet.Exceptions;

namespace WebUI.Areas.Solr.Controllers
{
    // http://wiki.apache.org/solr/CommonQueryParameters#fq
    // PENDIENTE: DrillDown
    [Authorize]
    public partial class SolrController : BaseController
    {
        private readonly ISolrOperations<SolrSeller> solr;
        private readonly string[] AllFacetFields = new[] { "paths", "attributes", "rating" };

        public SolrController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<SolrSeller>>();

            // Subir Redis y descomentar para usar session y caché
            //this.Session["Dato"] = 1;
            //var sessionData = Session["Dato"];

            //this.Cache.Set("Dato", 1);
            //var dato = this.Cache.Get<int>("Dato");
        }

        public class Parallel
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public bool Checked { get; set; }
        }

        // LISTA LOS SELLERS DE ACUERDO A LA BUSQUEDA INICIAL Y A LA SELECCION DE LOS CHECKBOX 'parallelItemDto'
        public ActionResult GetAllSellers(Parallel parallelItemDto)
        {
            SearchParameters parameters = null;

            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("SearchParametersCookie"))
            {
                var cookie = ControllerContext.HttpContext.Request.Cookies["SearchParametersCookie"];
                parameters = cookie.Value.FromJson<SearchParameters>();
            }

            if (parallelItemDto.Key != null)
            {
                var facet = parameters.BreadCrumb.FirstOrDefault(x => x.Key == parallelItemDto.Key.ToLower());

                if (facet.Key == null)
                {
                    if (parallelItemDto.Checked)
                    {
                        // TODO: Si está hay que hacer lo mismo que abajo para que no de error
                        parameters.BreadCrumb.Add(new KeyValuePair<string, string>(parallelItemDto.Key.ToLower(), parallelItemDto.Value));
                    }
                }
                else
                {
                    if (parallelItemDto.Checked)
                    {
                        // AGREGO
                        if (!parameters.BreadCrumb[facet.Key].Contains(parallelItemDto.Value))
                            parameters.BreadCrumb[facet.Key] = parameters.BreadCrumb[facet.Key] + "|" + parallelItemDto.Value;
                    }
                    else
                    {
                        // ELIMINO
                        if (parameters.BreadCrumb[facet.Key].Contains(parallelItemDto.Value))
                        {
                            parameters.BreadCrumb[facet.Key] = parameters.BreadCrumb[facet.Key].Replace("|" + parallelItemDto.Value, "").Replace(parallelItemDto.Value + "|", "").Replace(parallelItemDto.Value, "");
                            if (parameters.BreadCrumb[facet.Key] == "")
                            {
                                parameters.BreadCrumb.Remove(facet.Key);
                            }
                        }   
                    }
                }
            }

            var searchParametersCookie = new HttpCookie("SearchParametersCookie") { Value = parameters.ToJson() };
            ControllerContext.HttpContext.Response.Cookies.Add(searchParametersCookie);

            var fq = BuildFilterSelectedFacets(parameters);

            var sellers = solr.Query(BuildQuery(parameters), new QueryOptions
                                                                         {
                                                                             // Facetas ya seleccionadas -Drill Down-
                                                                             FilterQueries = fq,
                                                                             Rows = 5,
                                                                             Start = 0,
                                                                             SpellCheck = new SpellCheckingParameters()
                                                                         });

            return Json(new { sellers }, JsonRequestBehavior.AllowGet);
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

        // BUSQUEDA PARA FACETADO
        public ICollection<ISolrQuery> BuildFilterFacets(SearchParameters parameters)
        {
            var queriesFromFacets = parameters.Facets.Where(x => x.Value.Contains("|")).Select(p => (ISolrQuery)Query.Field(p.Key).In(p.Value.Split('|')));
            List<ISolrQuery> listFromFacets = queriesFromFacets.ToList();
            return listFromFacets;
        }

        // BUSQUEDA PARA LISTADO
        public ICollection<ISolrQuery> BuildFilterSelectedFacets(SearchParameters parameters)
        {
            var listFromParallel = parameters.BreadCrumb.Where(x => x.Value.Contains("|")).Select(p => (ISolrQuery)Query.Field(p.Key).In(p.Value.Split('|'))).ToList();
            var listFromFacet = parameters.BreadCrumb.Where(x => !x.Value.Contains("|")).Select(p => (ISolrQuery)Query.Field(p.Key).Is(p.Value)).ToList();

            listFromFacet.AddRange(listFromParallel);

            return listFromFacet;
        }

        public IEnumerable<string> SelectedFacetFields(SearchParameters parameters)
        {
            return parameters.Facets.Select(f => f.Key);
        }


        // El argumento parameters almacena los parametros de búsqueda en la vista, tanto el freesearch como las facetas seleccionadas
        // SE ENCARGA SOLO DEL ARMADO DEL FACETADO
        public ActionResult Index(SearchParameters parameters)
        {
            try
            {
                var start = (parameters.PageIndex - 1) * parameters.PageSize;

                var facetParameters = new FacetParameters();

                foreach (var data in AllFacetFields)
                {
                    ISolrFacetQuery facetQuery;

                    if (data == "paths")
                    {
                        string prefix = string.Empty;
                        var facet = parameters.Facets.SingleOrDefault(x => x.Key == "paths");

                         // AGREGA AL BREADCRUMB
                        foreach (var facetAux in parameters.Facets)
                        {
                            parameters.BreadCrumb.Add(new KeyValuePair<string, string>(facetAux.Key, facetAux.Value));
                        }

                        parameters.Facets.Clear();


                        if (facet.Key != null)
                        {
                            var depth = facet.Value.Split('/').Count() - 1 + "/";

                            prefix = facet.Value.Replace(facet.Value.Substring(0, facet.Value.IndexOf('/') + 1), depth);
                        }
                        else
                        { 
                            prefix = "0/";
                        }

                        facetQuery = new SolrFacetFieldQuery("paths") {MinCount = 1, Prefix = prefix };
                    }
                    else
                    {
                        facetQuery = new SolrFacetFieldQuery(data) { MinCount = 1 };
                    }

                   facetParameters.Queries.Add(facetQuery);
                }

               
                var searchParametersCookie = new HttpCookie("SearchParametersCookie") { Value = parameters.ToJson() };
                ControllerContext.HttpContext.Response.Cookies.Add(searchParametersCookie);

                 var queryOptions = new QueryOptions
                                       {
                                           FilterQueries = BuildFilterFacets(parameters),
                                           Rows = parameters.PageSize,
                                           Start = start,
                                           OrderBy = GetSelectedSort(parameters),
                                           SpellCheck = new SpellCheckingParameters(),
                                           Facet = facetParameters,
                                       };


                var matchingSellers = solr.Query(BuildQuery(parameters), queryOptions);

                var view = new SolrSellerViewModel
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
                return View(new SolrSellerViewModel
                {
                    QueryError = true,
                });
            }
        }

    }
}
