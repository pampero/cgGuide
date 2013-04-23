using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Common.Base;
using Framework.Solr.ViewModels;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.DSL;
using SolrNet.Exceptions;

namespace WebUI.Areas.Solr.Controllers
{
    public class SolrController : BaseController
    {
        private readonly ISolrOperations<Product> solr;
        private static readonly string[] AllFacetFields = new[] { "cat", "manu_exact" };

        public SolrController(ISolrOperations<Product> solr)
        {
            this.solr = solr;
            
        }

         public ActionResult AddProduct()
         {
             var product = new Product
             {
                 Id = new Random().Next().ToString(),
                 InStock = true,
                 Manufacturer = "Apple",
                 Price = 50m,
                 Weight = 20,
                 Categories = new[] { "Computers", "Electronics" }
             };

             solr.Add(product);
             solr.Commit();

             return Json(new { product }, JsonRequestBehavior.AllowGet);
         }


        public ActionResult RemoveAll()
        {
            var message = string.Empty;

            solr.Delete(SolrQuery.All);
            solr.Commit();

            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Remove(Product product)
         {
            var ok = false;

            try
            {
                solr.Delete(product.Id);
                solr.Commit();
                ok = true;
            }
            catch (Exception exception)
            {
                
            }
             
             return Json(new { ok }, JsonRequestBehavior.AllowGet);
         }

        public ActionResult GetAllProducts()
        {
            var products = solr.Query(new SolrQueryByRange<decimal>("price", 10m, 100m)).ToList();

            return Json(new { products }, JsonRequestBehavior.AllowGet);
        }

        

        public ISolrQuery BuildQuery(SearchParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.FreeSearch))
                return new SolrQuery(parameters.FreeSearch);
            return SolrQuery.All;
        }

        public ICollection<ISolrQuery> BuildFilterQueries(SearchParameters parameters)
        {
           
            var queriesFromFacets = from p in parameters.Facets
                                    select (ISolrQuery)Query.Field(p.Key).Is(p.Value);
            return queriesFromFacets.ToList();
        }

        public IEnumerable<string> SelectedFacetFields(SearchParameters parameters)
        {
            return parameters.Facets.Select(f => f.Key);
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

        public ActionResult Index(SearchParameters parameters)
        {
            try
            {
                var start = (parameters.PageIndex - 1) * parameters.PageSize;
                var matchingProducts = solr.Query(BuildQuery(parameters), new QueryOptions
                {
                    FilterQueries = BuildFilterQueries(parameters),
                    Rows = parameters.PageSize,
                    Start = start,
                    OrderBy = GetSelectedSort(parameters),
                    SpellCheck = new SpellCheckingParameters(),
                    Facet = new FacetParameters
                    {
                        Queries = AllFacetFields.Except(SelectedFacetFields(parameters))
                                                                              .Select(f => new SolrFacetFieldQuery(f) { MinCount = 1 })
                                                                              .Cast<ISolrFacetQuery>()
                                                                              .ToList(),
                    },
                });
                var view = new SolrViewModel
                {
                    Products = matchingProducts,
                    Search = parameters,
                    TotalCount = matchingProducts.NumFound,
                    Facets = matchingProducts.FacetFields//,
                  //  DidYouMean = GetSpellCheckingResult(matchingProducts),
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
