using System;

namespace Orckestra.Composer.Cart.Requests
{
    public class TokenizePaymentRequest
    {
        public string Token { get; set; }
        public Guid PaymentId { get; set; }
    }
}
