using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Cart.Parameters
{
    public class InitializePaymentParam : BaseCartParam
    {
        /// <summary>
        /// Id of the payment that will be initialized.
        /// </summary>
        public Guid PaymentId { get; set; }
        /// <summary>
        /// Type of the payment that will be initialized.
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// Additional data that may be used by the Overture Payment Provider to initialize the payment.
        /// This is optional.
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// Options that may be used to override default behaviors of the Overture Payment Provider.
        /// This is optional.
        /// </summary>
        public Dictionary<string, object> Options { get; set; }
    }
}
