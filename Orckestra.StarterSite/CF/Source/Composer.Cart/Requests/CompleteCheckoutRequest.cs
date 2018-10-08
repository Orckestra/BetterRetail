using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Cart.Requests
{
    public class CompleteCheckoutRequest
    {
        [Required]
        [Range(0, int.MaxValue - 1)]
        public int CurrentStep { get; set; }
    }
}
