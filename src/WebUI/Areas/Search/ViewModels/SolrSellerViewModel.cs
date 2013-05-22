using System;
using System.Collections.Generic;


namespace Framework.Solr.ViewModels
{
    public class SolrSellerViewModel
    {
        public SolrSellerViewModel()
        {
            Sellers = new List<SolrSeller>();
        }

        public SolrSeller SolrSeller { get; set; }

        public List<SolrSeller> Sellers { get; set; }
    }
}