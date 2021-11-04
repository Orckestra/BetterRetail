using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Cart.Requests
{
    public class GetGuestOrderRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string OrderNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
