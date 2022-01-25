using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class AddCreditCardParam : BaseCartParam
    {
        public string CardHolderName { get; set; }

        public Guid PaymentId { get; set; }

        public string VaultTokenId { get; set; }

        public string IpAddress { get; set; }

        public bool CreatePaymentProfile { get; set; }

        public string PaymentProviderName { get; set; }
    }
}
