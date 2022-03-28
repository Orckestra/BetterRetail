namespace Orckestra.Composer.ContentSearch.Request
{
    public class GetContentSearchResultsRequest
    {
        public string QueryString { get; set; }
        public string CurrentTabPathInfo { get; set; }
        public bool IsCurrentSiteOnly { get; set; }
    }
}
