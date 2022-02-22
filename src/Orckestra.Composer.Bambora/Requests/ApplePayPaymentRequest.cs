using Bambora.NA.SDK.Requests;
using Newtonsoft.Json;

namespace Orckestra.Composer.BamboraPayment.Requests
{
    public class ApplePayPaymentRequest: PaymentRequest
    {
        [JsonProperty(PropertyName = "apple_pay")]
        public ApplePay ApplePay { get; set; }

        public ApplePayPaymentRequest()
        {
            PaymentMethod = "apple_pay";
        }
    }
}
