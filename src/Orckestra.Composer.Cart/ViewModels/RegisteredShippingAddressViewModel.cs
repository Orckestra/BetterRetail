using System;
using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class RegisteredShippingAddressViewModel : BaseViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public Guid ShippingAddressId { get; set; }
    }
}
