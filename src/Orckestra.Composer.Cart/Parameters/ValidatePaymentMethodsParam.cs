using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class ValidatePaymentMethodParam : BaseCartParam
    {
        /// <summary>
        /// The Names of the providers to find Payment methods.
        /// Required
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// The Id of the payment method to validate.
        /// Required
        /// </summary>
        public Guid PaymentMethodId { get; set; }

        public bool IsAuthenticated { get; set; }
    }
}