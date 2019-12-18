using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class SetCheapestShippingMethodParam
    {
        public string BaseUrl { get; set; }

        public string Scope { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public Guid CustomerId { get; set; }

        public string CartName { get; set; }
    }
}