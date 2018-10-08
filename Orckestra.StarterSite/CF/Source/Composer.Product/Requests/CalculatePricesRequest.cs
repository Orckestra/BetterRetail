using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Product.Requests
{
    public class CalculatePricesRequest
    {
        [Required]
        [MinLength(1)]
        public string[] Products { get; set; }
    }
}
