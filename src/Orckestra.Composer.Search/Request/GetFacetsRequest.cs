using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Search.Request
{
    public class GetFacetsRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string QueryString { get; set; }
    }
}
