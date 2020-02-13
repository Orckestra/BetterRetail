using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class GetGuestOrderViewModel : BaseViewModel
    {
        [Required]
        public string OrderNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
