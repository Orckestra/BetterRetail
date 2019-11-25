using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class AddCreditCardParam
    {
        public string CardHolderName { get; set; }

        public string CartName { get; set; }

        public Guid CustomerId { get; set; }

        public string ScopeId { get; set; }

        public Guid PaymentId { get; set; }

        public string VaultTokenId { get; set; }

        public string IpAddress { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public bool CreatePaymentProfile { get; set; }

        public string PaymentProviderName { get; set; }
    }
}
