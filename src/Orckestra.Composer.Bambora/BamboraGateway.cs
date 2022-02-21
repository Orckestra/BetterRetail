using Bambora.NA.SDK;
using Bambora.NA.SDK.Exceptions;
using System;
using System.Configuration;

namespace Orckestra.Composer.BamboraPayment
{
    public class BamboraGateway
    {
        private readonly AppleGateway _gateway;

        public BamboraGateway()
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

            _gateway = new AppleGateway()
            {
                MerchantId = merchantId,
                AppleMerchantId = applePayMerchantId,
                PaymentsApiPasscode = bamboraPaymentsApiPasscode,
                ApiVersion = "1"
            };
        }

        public PaymentResponse PreAuth(decimal amount, string paymenToken, bool complete = false)
        {
           var request = new Requests.PaymentApplePayRequest()
            {
                Amount = amount,
                PaymentMethod = "apple_pay",
                ApplePay = new Requests.ApplePay()
                {
                    MerchantId = _gateway.AppleMerchantId,
                    PaymenToken = Base64Encode(paymenToken),
                    Complete = complete
                }
            };

            try
            {
                return _gateway.Payments.MakePayment(request);
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
