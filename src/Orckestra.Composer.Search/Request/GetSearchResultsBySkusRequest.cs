namespace Orckestra.Composer.Search.Request
{
    public class GetSearchResultsBySkusRequest
    {
        public string[] Skus { get; set; }

        public string QueryString { get; set; }

        public bool IncludeFacets { get; set; }
    }
}
