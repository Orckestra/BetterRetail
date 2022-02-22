using Bambora.NA.SDK;
using Bambora.NA.SDK.Exceptions;
using System;
using System.Configuration;

namespace Orckestra.Composer.BamboraPayment
{
    public class BamboraApplePayGateway
    {
        private readonly AppleGateway _applePayGateway;

        public BamboraApplePayGateway()
        {
            if(!int.TryParse(ConfigurationManager.AppSettings["BamboraMerchantId"], out int merchantId))
            {
                throw new ArgumentException(nameof(merchantId));
            }

            var applePayMerchantId = ConfigurationManager.AppSettings["ApplePayMerchantId"];
            if (string.IsNullOrWhiteSpace(applePayMerchantId))
            {
                throw new ArgumentException(nameof(applePayMerchantId));
            }

            var bamboraPaymentsApiPasscode = ConfigurationManager.AppSettings["BamboraPaymentsApiPasscode"];
            if (string.IsNullOrWhiteSpace(applePayMerchantId))
            {
                throw new ArgumentException(nameof(bamboraPaymentsApiPasscode));
            }

            _applePayGateway = new AppleGateway()
            {
                MerchantId = merchantId,
                AppleMerchantId = applePayMerchantId,
                PaymentsApiPasscode = bamboraPaymentsApiPasscode,
                ApiVersion = "1"
            };
        }

        public PaymentResponse PreAuth(decimal amount, string paymenToken, bool complete = false)
        {
           var request = new Requests.ApplePayPaymentRequest()
            {
                Amount = amount, //should come from Payment object
                ApplePay = new Requests.ApplePay()
                {
                    MerchantId = _applePayGateway.AppleMerchantId,
                    PaymenToken = Base64Encode(paymenToken),
                    Complete = complete
                }
            };

            try
            {
                return _applePayGateway.Payments.MakePayment(request);
            }
            catch (Exception ex)
            {

                var exception = ex as BusinessRuleException;

                if (exception != null)
                {
                    //
                }

                return new PaymentResponse
                {
                    Approved = "Declined",
                    Message = ex.Message,
                };
            }
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes, Base64FormattingOptions.None);
        }
    }
}
