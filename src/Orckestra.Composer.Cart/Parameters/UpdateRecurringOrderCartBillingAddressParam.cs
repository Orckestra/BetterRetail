using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateRecurringOrderCartBillingAddressParam : BaseCartParam
    {
        public Guid BillingAddressId { get; set; }
        public string BaseUrl { get; set; }
        public bool UseSameForShippingAndBilling { get; set; }
    }
}
