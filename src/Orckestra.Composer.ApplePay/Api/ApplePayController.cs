using Orckestra.Composer.ApplePay.Parameters;
using Orckestra.Composer.ApplePay.Requests;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Request;
using Composite.Core;

namespace Orckestra.Composer.ApplePay.Api
{
    public class ApplePayController : ApiController
    {
        private readonly ApplePayClient _client;
        private readonly MerchantCertificate _certificate;

        public ApplePayController()
        {
            _client = new ApplePayClient();
            _certificate = new MerchantCertificate(new Options.ApplePayOptions());
        }

        /// <summary>
        /// Get the shopping cart for the current customer
        /// </summary>
        /// <returns>A Json representation of cart state</returns>
        [HttpPost]
        [ActionName("create")]
        public async Task<IHttpActionResult> CreateSession(CreateApplePaySessionParam param)
        {
            if (!ModelState.IsValid ||
            string.IsNullOrWhiteSpace(param?.ValidationUrl) ||
            !Uri.TryCreate(param.ValidationUrl, UriKind.Absolute, out Uri requestUri))
            {
                return BadRequest();
            }

            var request = new ApplePaySessionRequest();
            request.DisplayName = "Pay for Better Retail Order";
            request.Initiative = "web";
            request.InitiativeContext = Request.Headers.Host;
            request.MerchantIdentifier = _certificate.GetMerchantIdentifier();

            var merchantSession = await _client.GetMerchantSessionAsync(requestUri, request);

            return Json(merchantSession);
        }
    }
}