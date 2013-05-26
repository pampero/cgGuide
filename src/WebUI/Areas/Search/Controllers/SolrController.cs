using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Common.Base;
using Framework.Solr.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model;
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
    public class SolrController : BaseController
    {
        private readonly ISolrOperations<SolrSeller> solr;
        private readonly string[] AllFacetFields = new[] { "categories", "attributes", "rating" }; //"subcategories",

        public SolrController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<SolrSeller>>();

            // Subir Redis y descomentar para usar session y caché
            //this.Session["Dato"] = 1;
            //var sessionData = Session["Dato"];

            //this.Cache.Set("Dato", 1);
            //var dato = this.Cache.Get<int>("Dato");
        }

        public class SearchParametersDto
        {
            public string FreeSearch { get; set; }
            public List<Facet> Facets { get; set; }
            public List<Parallel> Parallels { get; set; }
            //  public List<FacetCollection> Facets { get; set; }
            //    public List<ParallelCollection> Parallels { get; set; }

            public SearchParametersDto()
            {
                Facets = new List<Facet>();
                Parallels = new List<Parallel>();
            }

        }

        public class FacetCollection
        {
            public string Key { get; set; }
            public List<Facet> Facets { get; set; }
        }

        public class ParallelCollection
        {
            public string Key { get; set; }
            public List<Parallel> Parallels { get; set; }
        }

        public class Parallel
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Facet
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public ICollection<ISolrQuery> BuildParallelFilterQueries(SearchParameters parameters)
        {
            parameters.Parallels.Add(new KeyValuePair<string, string>("rating", "3"));
            parameters.Parallels.Add(new KeyValuePair<string, string>("rating", "4"));

            var queriesFromParallels = parameters.Parallels.Select(p => (ISolrQuery)Query.Field(p.Key).In(new int[] { 3, 4 }));

            return queriesFromParallels.ToList();
        }

        // Debería enviar el SearchParameters con el estado actual, posiblemente con un fq sobre Parallels
        //public ActionResult GetAllSellers(SearchParametersDto searchParametersDto)
        public ActionResult GetAllSellers(List<Parallel> parallelsDto)
        {
            var parameters = (SearchParameters)Session["Parameters"];


            if ((parallelsDto != null) && (parallelsDto.Count > 0))
            {
                var firstPass = true;
                var currentKey = "";

                foreach (var parallel in parallelsDto.OrderBy(x=>x.Key))
                {
                    var facet = parameters.Facets.FirstOrDefault(x => x.Key == parallel.Key.ToLower());

                    if (facet.Key == null)
                    {
                        parameters.Facets.Add(new KeyValuePair<string, string>(parallel.Key.ToLower(), parallel.Value));
                    }
                    else
                    {
                        if (currentKey != parallel.Key.ToLower())
                        {
                            currentKey = parallel.Key.ToLower();
                            firstPass = true;
                        }
                    
                        if (firstPass)
                        {
                            firstPass = false;
                            parameters.Facets[facet.Key] = parallel.Value;
                        }
                        else
                        {
                            if (!parameters.Facets[facet.Key].Contains(parallel.Value))
                                parameters.Facets[facet.Key] = parameters.Facets[facet.Key] + "|" + parallel.Value;
                        }
                    }
                }
            }

            Session["Parameters"] = parameters;

            var facetParameters = new FacetParameters
                                          {
                                              Queries = AllFacetFields.Except(SelectedFacetFields(parameters))
                                                  .Select(f => new SolrFacetFieldQuery(f) { MinCount = 1 })
                                                  .Cast<ISolrFacetQuery>()
                                                  .ToList(),
                                          };

            var sellers = solr.Query(BuildQuery(parameters), new QueryOptions
                                                                         {
                                                                             // Facetas ya seleccionadas -Drill Down-
                                                                             FilterQueries =
                                                                                 BuildFilterQueries(parameters),
                                                                             Rows = 5,
                                                                             Start = 0,
                                                                             SpellCheck = new SpellCheckingParameters(),
                                                                             Facet = facetParameters
                                                                         });


            return Json(new { sellers }, JsonRequestBehavior.AllowGet);
        }



        public ISolrQuery BuildQuery(SearchParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.FreeSearch))
                return new SolrQuery(parameters.FreeSearch);
            return SolrQuery.All;
        }


        public SortOrder[] GetSelectedSort(SearchParameters parameters)
        {
            return new[] { SortOrder.Parse(parameters.Sort) }.Where(o => o != null).ToArray();
        }

        //private string GetSpellCheckingResult(ISolrQueryResults<Product> products)
        //{
        //    return string.Join(" ", products.SpellChecking
        //                                .Select(c => c.Suggestions.FirstOrDefault())
        //                                .Where(c => !string.IsNullOrEmpty(c))
        //                                .ToArray());
        //}





        public ICollection<ISolrQuery> BuildFilterQueries(SearchParameters parameters)
        {
            var queriesFromParallels = parameters.Facets.Where(x => x.Value.Contains("|")).Select(p => (ISolrQuery)Query.Field(p.Key).In(p.Value.Split('|')));

            var queriesFromFacets = parameters.Facets.Where(x => !x.Value.Contains("|")).Select(p => (ISolrQuery)Query.Field(p.Key).Is(p.Value));

            List<ISolrQuery> listFromFacet = queriesFromFacets.ToList();
            List<ISolrQuery> listFromParallel = queriesFromParallels.ToList();

            listFromFacet.AddRange(listFromParallel);

            return listFromFacet;
        }


        public IEnumerable<string> SelectedFacetFields(SearchParameters parameters)
        {
            return parameters.Facets.Select(f => f.Key);
        }

        // El argumento parameters almacena los parametros de búsqueda en la vista, tanto el freesearch como las facetas seleccionadas
        public ActionResult Index(SearchParameters parameters)
        {
            var facet = parameters.Facets.FirstOrDefault(x => x.Key == "categories");

            if (facet.Key != null)
                AllFacetFields[0] = "subcategories";
            else
                AllFacetFields[0] = "categories";


            Session["Parameters"] = parameters;

            try
            {
                var start = (parameters.PageIndex - 1) * parameters.PageSize;

                // Saca de AllFacetFields las facetas seleccionadas desde la vista, luego crea un campo SolrFacetField por cada faceta y lo convierte a lista.
                var facetParameters = new FacetParameters
                                          {
                                              Queries = AllFacetFields.Except(SelectedFacetFields(parameters))
                                                  .Select(f => new SolrFacetFieldQuery(f) { MinCount = 1 })
                                                  .Cast<ISolrFacetQuery>()
                                                  .ToList(),
                                          };

                var matchingSellers = solr.Query(BuildQuery(parameters), new QueryOptions
                {
                    // Facetas ya seleccionadas -Drill Down-
                    FilterQueries = BuildFilterQueries(parameters),
                    Rows = parameters.PageSize,
                    Start = start,
                    OrderBy = GetSelectedSort(parameters),
                    SpellCheck = new SpellCheckingParameters(),
                    Facet = facetParameters
                });

                var view = new SolrSellerViewModel
                {
                    Sellers = matchingSellers,
                    Search = parameters,
                    TotalCount = matchingSellers.NumFound,
                    Facets = matchingSellers.FacetFields//,
                    //  DidYouMean = GetSpellCheckingResult(matchingSellers),
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

        // public ActionResult AddProduct()
        // {
        //     var product = new Product
        //     {
        //         Id = new Random().Next().ToString(),
        //         InStock = true,
        //         Manufacturer = "Apple",
        //         Price = 50m,
        //         Weight = 20,
        //         Categories = new[] { "Computers", "Electronics" }
        //     };

        //     solr.Add(product);
        //     solr.Commit();

        //     return Json(new { product }, JsonRequestBehavior.AllowGet);
        // }


        //public ActionResult RemoveAll()
        //{
        //    var message = string.Empty;

        //    solr.Delete(SolrQuery.All);
        //    solr.Commit();

        //    return Json(new { message }, JsonRequestBehavior.AllowGet);
        //}


        //public ActionResult Remove(Product product)
        // {
        //    var ok = false;

        //    try
        //    {
        //        solr.Delete(product.Id);
        //        solr.Commit();
        //        ok = true;
        //    }
        //    catch (Exception exception)
        //    {

        //    }

        //     return Json(new { ok }, JsonRequestBehavior.AllowGet);
        // }


    }
}
