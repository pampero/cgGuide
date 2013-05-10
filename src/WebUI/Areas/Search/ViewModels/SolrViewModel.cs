using System;
using System.Collections.Generic;


namespace Framework.Solr.ViewModels
{
    public class ProductsViewModel
    {
        public ProductsViewModel()
        {
            Products = new List<Product>();
        }

        public Product Product { get; set; }

        public List<Product> Products { get; set; }
    }
}