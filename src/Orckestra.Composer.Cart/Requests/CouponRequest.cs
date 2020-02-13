using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Cart.Requests
{
    public class CouponRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string CouponCode { get; set; }
    }
}
