using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CustomerSummaryViewModel : BaseViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
