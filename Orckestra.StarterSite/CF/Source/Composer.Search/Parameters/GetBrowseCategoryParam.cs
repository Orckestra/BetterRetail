using System.Web;

namespace Orckestra.Composer.Search.Parameters
{
    public class GetBrowseCategoryParam
    {
        public string Keywords { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
        public int Page { get; set; }
        public string CategoryId { get; set; }

        public HttpRequestBase Request { get; set; }
    }
}