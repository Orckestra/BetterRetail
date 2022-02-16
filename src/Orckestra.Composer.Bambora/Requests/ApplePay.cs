using Newtonsoft.Json;

namespace Orckestra.Composer.BamboraPayment.Requests
{
    public class ApplePay
    {
        [JsonProperty(PropertyName = "apple_pay_merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// apple_pay_base64_encoded_token
        /// </summary>
        [JsonProperty(PropertyName = "payment_token")]
        public string PaymenToken { get; set; }

        /// <summary>
        /// <true(Defaultstotrueifomitted.Usedforapurchase)|false(UsedforaPre-Auth.)>}}
        /// </summary>
        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }
    }
}
