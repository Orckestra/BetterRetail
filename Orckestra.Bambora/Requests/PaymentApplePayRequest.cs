using Newtonsoft.Json;

namespace Orckestra.Bambora.Requests
{
    public class PaymentApplePayRequest
    {
        /// <summary>
		/// This is set automatically by the PaymentsAPI class. You do not need to set it.
		/// </summary>
		[JsonProperty(PropertyName = "payment_method")]
        public string PaymentMethod { get; set; } = "apple_pay";

        /// <summary>
        /// Include a unique order reference number. 30 alphanumeric (a/n) characters.
        /// Optional
        /// </summary>
        [JsonProperty(PropertyName = "order_number")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// In the format 0.00. Max 2 decimal places. Max 9 digits total.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "apple-pay")]
        public ApplePay ApplePay { get; set; }

    }
}
