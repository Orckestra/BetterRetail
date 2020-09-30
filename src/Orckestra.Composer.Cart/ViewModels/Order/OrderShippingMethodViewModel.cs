using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class OrderShippingMethodViewModel : BaseViewModel
    {
        /// <summary>
        /// The Shipping Method UI-friendly display name.
        /// In the summary it is the localized text for L_ShippingBasedOn and L_ShippingBasedOnNonTaxable
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The Shipping Method Cost.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string Cost { get; set; }

        /// <summary>
        /// Indicate if the shipping is taxable 
        /// </summary>
        public bool Taxable { get; set; }

        public FulfillmentMethodType FulfillmentMethodType { get; set; }
    }
}
