using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateRecurringOrderCartShippingAddressParam : BaseCartParam
    {
        public Guid ShippingAddressId { get; set; }
        public Guid BillingAddressId { get; set; }
        public string BaseUrl { get; set; }
        public bool UseSameForShippingAndBilling { get; set; }
    }
}
