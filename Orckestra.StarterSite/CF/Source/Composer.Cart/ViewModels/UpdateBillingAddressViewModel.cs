using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class UpdateBillingAddressViewModel : BaseViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string PostalCode { get; set; }
    }
}
