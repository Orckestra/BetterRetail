using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateRecurringOrderCartsAddressesParam : BaseCartParam
    {
        public Guid AddressId { get; set; }
        public string BaseUrl { get; set; }
    }
}
