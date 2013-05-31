using System.Collections.Generic;
using Model.Enums;
using SolrNet.Attributes;

namespace Framework.Solr.ViewModels
{
    public class SolrBusiness
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("business")]
        public string Name { get; set; }

        [SolrField("city")]
        public string City { get; set; }

        [SolrField("attributes")] 
        public ICollection<string> Attributes { get; set; }

        [SolrField("categories")]
        public string Categories { get; set; }

        [SolrField("paths")]
        public ICollection<string> Paths { get; set; }

        [SolrField("rating")]
        public string Rating { get; set; }

        [SolrField("latlng")]
        public string LatLng { get; set; }
    }
}
