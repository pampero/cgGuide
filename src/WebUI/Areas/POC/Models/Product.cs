using System.Collections.Generic;
using SolrNet.Attributes;

namespace Framework.Solr.ViewModels
{
    public class Product
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("manu")]
        public string Manufacturer { get; set; }

        [SolrField("cat")] // cat is a multiValued field
        public ICollection<string> Categories { get; set; }

        [SolrField("price")]
        public decimal Price { get; set; }

        [SolrField("inStock")]
        public bool InStock { get; set; }

        //[SolrField("timestamp")]
        //public DateTime Timestamp { get; set; }

        [SolrField("weight")]
        public double? Weight { get; set; } // nullable property, it might not be defined on all documents.
    }
}
