using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Common.Base;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Solr;
using SolrNet;

namespace WebUI.Areas.Solr.Controllers
{
    public class SolrController : BaseController
    {
        private readonly ISolrOperations<Product> solr;

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

        public ActionResult Index()
        {
            return View();
        }

    }
}
