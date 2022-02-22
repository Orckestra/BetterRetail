using Bambora.NA.SDK.Requests;
using Newtonsoft.Json;

namespace Orckestra.Composer.BamboraPayment.Requests
{
    public class PaymentApplePayRequest: PaymentRequest
    {
        [JsonProperty(PropertyName = "apple_pay")]
        public ApplePay ApplePay { get; set; }

    }
}
