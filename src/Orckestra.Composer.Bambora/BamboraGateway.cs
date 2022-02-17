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
             _gateway = new AppleGateway()
            {
                MerchantId = int.Parse(ConfigurationManager.AppSettings["BamboraMerchantId"]),
                AppleMerchantId = ConfigurationManager.AppSettings["ApplePayMerchantId"],
                PaymentsApiPasscode = ConfigurationManager.AppSettings["BamboraPaymentsApiPasscode"],
                ApiVersion = "1"
            };
        }

        public PaymentResponse PreAuth(decimal amount, string orderNumber, string paymenToken, bool complete = false)
        {
           var request = new Requests.PaymentApplePayRequest()
            {
                Amount = amount,
                OrderNumber = orderNumber,
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
                    OrderNumber = orderNumber,
                    Message = ex.Message,
                };
            }
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
