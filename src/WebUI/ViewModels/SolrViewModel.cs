using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;

namespace Framework.ViewModels
{
    public class SolrViewModel
    {
        public SolrViewModel()
        {
            Products = new List<Product>();
        }

        public Product Product { get; set; }

        public List<Product> Products { get; set; }
    }
}