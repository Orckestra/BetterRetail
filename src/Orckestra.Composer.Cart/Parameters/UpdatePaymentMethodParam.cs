using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdatePaymentMethodParam : BaseCartParam
    {
        /// <summary>
        /// Id of the payment that will be updated.
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// ID of the selected Payment Method.
        /// </summary>
        public Guid PaymentMethodId { get; set; }

        /// <summary>
        /// Name of the Overture Payment Provider supplying the PaymentMethod.
        /// </summary>
        public string PaymentProviderName { get; set; }
        /// <summary>
        /// Type of the Overture Payment supplying the PaymentMethod.
        /// </summary>
        public string PaymentType { get; set; }

        public bool IsAuthenticated { get; set; }

        public List<string> ProviderNames { get; set; }

        public UpdatePaymentMethodParam()
        {
            ProviderNames = new List<string>();
        }
    }
}
