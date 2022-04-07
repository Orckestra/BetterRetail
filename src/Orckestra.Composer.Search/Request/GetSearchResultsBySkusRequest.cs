using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Search.Request
{
    public class GetSearchResultsBySkusRequest
    {
        [Required]
        public string[] Skus { get; set; }

        public string QueryString { get; set; }

        public bool IncludeFacets { get; set; }
    }
}
