using System.Collections.Generic;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetPaymentMethodsParam : BaseCartParam
    {
        /// <summary>
        /// The Names of the providers to find Payment methods.
        /// Required
        /// </summary>
        public List<string> ProviderNames { get; set; }

        public bool IsAuthenticated { get; set; }

        public GetPaymentMethodsParam()
        {
            ProviderNames = new List<string>();
        }
    }
}