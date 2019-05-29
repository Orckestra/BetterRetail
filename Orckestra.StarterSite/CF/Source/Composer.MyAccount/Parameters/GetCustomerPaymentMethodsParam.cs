using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    public class GetCustomerPaymentMethodsParam
    {
        /// <summary>
        /// Id of the customer.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The ScopeId where to find the Payment methods.
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The culture for returned Payment methods info.
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The Names of the providers to find Payment methods.
        /// Required
        /// </summary>
        public List<string> ProviderNames { get; set; }

        public GetCustomerPaymentMethodsParam()
        {
            ProviderNames = new List<string>();
        }
    }
}
