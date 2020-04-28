using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class ShippingMethodTypeViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the Fulfillment method type it belongs to
        /// </summary>
        public FulfillmentMethodType FulfillmentMethodType { get; set; }

        /// <summary>
        /// Gets or sets the Fulfillment method string type it belongs to
        /// </summary>
        public string FulfillmentMethodTypeString { get; set; }

        /// <summary>
        /// The Shipping Method UI-friendly display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets shipping methods
        /// </summary>
        public IList<ShippingMethodViewModel> ShippingMethods { get; set; }

        public ShippingMethodTypeViewModel()
        {
            ShippingMethods = new List<ShippingMethodViewModel>();
        }
    }
}