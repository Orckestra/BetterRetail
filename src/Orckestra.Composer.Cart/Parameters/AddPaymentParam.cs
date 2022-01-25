using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Cart.Parameters
{
    public class AddPaymentParam : BaseCartParam
    {
        /// <summary>
        /// Amount to authorize in this payment. This parameter is optional.
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Address to use as a billing address. This parameter is optional.
        /// </summary>
        public Address BillingAddress { get; set; }
    }
}
