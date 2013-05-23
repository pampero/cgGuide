using System.Collections.Generic;
using Model.Enums;
using SolrNet.Attributes;

namespace Framework.Solr.ViewModels
{
    public class SolrSeller
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("seller")]
        public string Name { get; set; }

        [SolrField("city")]
        public string City { get; set; }

        [SolrField("attributes")] 
        public ICollection<string> Attributes { get; set; }

        [SolrField("subcategories")]
        public ICollection<string> Subcategories { get; set; }

        [SolrField("categories")]
        public ICollection<string> Categories { get; set; }

        [SolrField("rating")]
        public Rating Rating { get; set; }

        [SolrField("latlng")]
        public string LatLng { get; set; }
    }
}
