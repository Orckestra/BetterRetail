using Newtonsoft.Json;

namespace Orckestra.Composer.ApplePay.Requests
{
    public class ApplePaySessionRequest
    {
        [JsonProperty(PropertyName = "merchantIdentifier")]
        public string MerchantIdentifier { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "initiative")]
        public string Initiative { get; set; }

        [JsonProperty(PropertyName = "initiativeContext")]
        public string InitiativeContext { get; set; }
    }
}
