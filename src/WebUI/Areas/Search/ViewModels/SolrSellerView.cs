using System.Collections.Generic;

namespace Framework.Solr.ViewModels
{
    public class SolrViewModel
    {
        public SearchParameters Search { get; set; }
        public ICollection<SolrSeller> Sellers { get; set; }
        public int TotalCount { get; set; }
        public IDictionary<string, ICollection<KeyValuePair<string, int>>> Facets { get; set; }
        public string DidYouMean { get; set; }
        public bool QueryError { get; set; }

        public SolrViewModel()
        {
            Search = new SearchParameters();
            Facets = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
            Sellers = new List<SolrSeller>();
        }
    }
}