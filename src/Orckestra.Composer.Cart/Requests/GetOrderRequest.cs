using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Cart.Requests
{
    public class GetOrderRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string OrderNumber { get; set; }
    }
}
