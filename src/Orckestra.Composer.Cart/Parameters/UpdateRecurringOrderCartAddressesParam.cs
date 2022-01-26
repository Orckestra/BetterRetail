using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateRecurringOrderCartsAddressesParam
    {
        public string ScopeId { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public Guid AddressId { get; set; }
        public Guid CustomerId { get; set; }
        public string BaseUrl { get; set; }
    }
}
