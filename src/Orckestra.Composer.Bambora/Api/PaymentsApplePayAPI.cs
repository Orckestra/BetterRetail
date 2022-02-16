using Bambora.NA.SDK;
using Bambora.NA.SDK.Data;
using Bambora.NA.SDK.Requests;
using Newtonsoft.Json;
using Orckestra.Composer.BamboraPayment.Requests;
using System;

namespace Orckestra.Composer.BamboraPayment.Api
{
    public class PaymentsApplePayAPI
    {

        private Bambora.NA.SDK.Configuration _configuration;
        private IWebCommandExecuter _webCommandExecuter = new WebCommandExecuter();

        public Bambora.NA.SDK.Configuration Configuration
        {
            set { _configuration = value; }
        }

        public IWebCommandExecuter WebCommandExecuter
        {
            set { _webCommandExecuter = value; }
        }

        /// <summary>
		/// Make a apple pay payment.
		/// </summary>
		/// <returns>the payment result</returns>
		/// <param name="paymentRequest">Payment request.</param>
		public PaymentResponse MakePayment(PaymentApplePayRequest paymentRequest)
        {

            Gateway.ThrowIfNullArgument(paymentRequest, "paymentRequest");

            string url = BamboraUrls.BasePaymentsUrl
                            .Replace("{v}", String.IsNullOrEmpty(_configuration.Version) ? "v1" : "v" + _configuration.Version)
                               .Replace("{p}", String.IsNullOrEmpty(_configuration.Platform) ? "www" : _configuration.Platform);


            HttpsWebRequest req = new HttpsWebRequest()
            {
                MerchantId = _configuration.MerchantId,
                Passcode = _configuration.PaymentsApiPasscode,
                WebCommandExecutor = _webCommandExecuter
            };

            string response = req.ProcessTransaction(HttpMethod.Post, url, paymentRequest);

            return JsonConvert.DeserializeObject<PaymentResponse>(response);

        }
    }
}
