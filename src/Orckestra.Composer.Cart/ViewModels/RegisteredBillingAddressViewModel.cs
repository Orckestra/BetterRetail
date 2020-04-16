using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class RegisteredBillingAddressViewModel : BaseViewModel
    {
        /// <summary>
        /// The BillingAddress Id
        /// </summary>
        public Guid BillingAddressId { get; set; }

        /// <summary>
        /// Indicate if the BillingAddress informations come from the ShippingAddress.
        /// </summary>
        public bool UseShippingAddress { get; set; }
    }
}
