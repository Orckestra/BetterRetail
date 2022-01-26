﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateRecurringOrderCartShippingAddressParam
    {
        public string ScopeId { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string CartName { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid BillingAddressId { get; set; }
        public Guid CustomerId { get; set; }
        public string BaseUrl { get; set; }
        public bool UseSameForShippingAndBilling { get; set; }
    }
}
