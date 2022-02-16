using Bambora.NA.SDK;
using Bambora.NA.SDK.Exceptions;
using System;

namespace Orckestra.Composer.BamboraPayment
{
    public class BamboraGateway
    {
        private readonly AppleGateway _gateway;

        public BamboraGateway()
        {
 
            _gateway = new AppleGateway()
            {
                MerchantId = 300205295,
                AppleMerchantId = "merchant.wfecm.int.platform.orckestra.cloud",
                PaymentsApiKey = "B8A8AA0A947B424794D3CD67E7D52582",
                ApiVersion = "1"
            };
        }

        public PaymentResponse PreAuth(decimal amount, string orderNumber, string paymenToken, bool complete = false)
        {
           var request = new Requests.PaymentApplePayRequest()
            {
                Amount = amount,
                OrderNumber = orderNumber,
                ApplePay = new Requests.ApplePay()
                {
                    MerchantId = _gateway.AppleMerchantId,
                    PaymenToken = paymenToken,
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
    }
}
