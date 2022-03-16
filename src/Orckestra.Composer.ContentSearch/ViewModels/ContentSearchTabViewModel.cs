using Orckestra.Search.WebsiteSearch;

namespace Orckestra.Composer.ContentSearch.ViewModels
{
    public class ContentSearchTabViewModel
    {
        public string UrlTitle { get; set; }
        public string TabUrl { get; set; }
        public string Title { get; set; }
        public WebsiteSearchResult SearchResults { get; set; }
        public long Total { get; set; }
        public int PagesCount { get; set; }
        public bool IsActive { get; set; }
        public string DataTypes { get; set; }
    }
}
