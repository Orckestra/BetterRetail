using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class UpdateShippingAddressViewModel : BaseViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string PostalCode { get; set; }
    }
}
