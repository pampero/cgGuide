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
    [Authorize]
    public class SolrController : BaseController
    {
        private readonly ISolrOperations<SolrSeller> solr;
        private static readonly string[] AllFacetFields = new[] { "categories", "subcategories", "attributes" };

        public SolrController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<SolrSeller>>();

            // Subir Redis y descomentar para usar session y caché
            //this.Session["Dato"] = 1;
            //var sessionData = Session["Dato"];

            //this.Cache.Set("Dato", 1);
            //var dato = this.Cache.Get<int>("Dato");
        }


        public ActionResult ChangeLanguage()
        {
            const string Message = "Lenguaje cambiado!";

            var ci = new CultureInfo("es-AR");

            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
 
            return Json(new { Message }, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetAllSellers()
        {
            var sellers = solr.Query(new SolrQueryByField("seller", "Seller 1")).ToList();

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

        public ICollection<ISolrQuery> BuildFilterQueries(IDictionary<string, string> facets)
        {
            var queriesFromFacets = facets.Select(p => (ISolrQuery)Query.Field(p.Key).Is(p.Value));
            return queriesFromFacets.ToList();
        }

        public IEnumerable<string> SelectedFacetFields(SearchParameters parameters)
        {
            return parameters.Facets.Select(f => f.Key);
        }

        // El argumento parameters almacena los parametros de búsqueda en la vista, tanto el freesearch como las facetas seleccionadas
        public ActionResult Index(SearchParameters parameters)
        {
            try
            {
                var start = (parameters.PageIndex - 1) * parameters.PageSize;

                // Saca de AllFacetFields las facetas seleccionadas desde la vista, luego crea un campo SolrFacetField por cada faceta y lo convierte a lista.
                var facetParameters = new FacetParameters
                                          {
                                              Queries = AllFacetFields.Except(SelectedFacetFields(parameters))
                                                  .Select(f => new SolrFacetFieldQuery(f) {MinCount = 1})
                                                  .Cast<ISolrFacetQuery>()
                                                  .ToList(),
                                          };

                var matchingSellers = solr.Query(BuildQuery(parameters), new QueryOptions
                {
                    // Facetas ya seleccionadas -Drill Down-
                    FilterQueries = BuildFilterQueries(parameters.Facets),
                    Rows = parameters.PageSize,
                    Start = start,
                    OrderBy = GetSelectedSort(parameters),
                    SpellCheck = new SpellCheckingParameters(),
                    Facet = facetParameters
                });

                var view = new SolrViewModel
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
                return View(new SolrViewModel
                {
                    QueryError = true,
                });
            }
        }



    }
}
