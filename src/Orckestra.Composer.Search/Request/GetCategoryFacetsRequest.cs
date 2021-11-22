using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Search.Request
{
    public class GetCategoryFacetsRequest
    {
        public string QueryString { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CategoryId { get; set; }
    }
}
