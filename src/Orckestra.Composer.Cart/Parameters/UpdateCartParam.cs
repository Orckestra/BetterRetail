using System.Collections.Generic;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateCartParam : BaseCartParam
    {
        /// <summary>
        /// The currency ISO code under which the items are sold 'Currency.IsoCode'
        /// </summary>
        public string BillingCurrency { get; set; }

        /// <summary>
        /// The collection of coupon codes included in the order
        /// </summary>
        public List<Coupon> Coupons { get; set; }

        /// <summary>
        /// The customer information
        /// </summary>
        public CustomerSummary Customer { get; set; }

        /// <summary>
        /// The order location information
        /// </summary>
        public OrderLocationSummary OrderLocation { get; set; }

        /// <summary>
        /// The bag containing all the custom attributes
        /// </summary>
        public PropertyBag PropertyBag { get; set; }

        /// <summary>
        /// The collection of shipments associated to this order
        /// </summary>
        public List<Shipment> Shipments { get; set; }

        /// <summary>
        /// The status of the cart
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The payments in the cart
        /// </summary>
        public List<Payment> Payments { get; set; }

        public UpdateCartParam()
        {
            Coupons = new List<Coupon>();
            Shipments = new List<Shipment>();
            Payments = new List<Payment>();
        }
    }
}
