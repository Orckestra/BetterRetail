using System;
using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class ShippingMethodViewModel : BaseViewModel
    {
        /// <summary>
        /// The Shipping Method UI-friendly display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The Shipping Method Cost.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string Cost { get; set; }

        /// <summary>
        /// The Shipping Method Cost.
        /// </summary>
        [MapTo("Cost")]
        public double CostDouble { get; set; }

        /// <summary>
        /// The expected number days before Shipping.
        /// </summary>
        public string ExpectedDaysBeforeDelivery { get; set; }

        /// <summary>
        /// The Shipping Method unique Identifier.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// The Shipping Provider unique Identifier.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public Guid ShippingProviderId { get; set; }
        
        /// <summary>
        /// Gets or sets the Fulfillment method type it belongs to
        /// </summary>
        public FulfillmentMethodType FulfillmentMethodType { get; set; }

        /// <summary>
        /// Gets or sets the Fulfillment method string type it belongs to
        /// </summary>
        public string FulfillmentMethodTypeString { get; set; }

        /// <summary>
        /// Gets or sets the flag to indicate if the shipping method is a ship to store
        /// </summary>
        public bool IsShipToStoreType { get; set; }

        /// <summary>
        /// Gets or sets the flag to indicate if the shipping method is default selected
        /// </summary>
        public bool IsSelected { get; set; }
    }
}