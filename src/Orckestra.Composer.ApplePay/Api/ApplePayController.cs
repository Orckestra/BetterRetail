using Composite.Core;
using Orckestra.Composer.ApplePay.Parameters;
using Orckestra.Composer.ApplePay.Requests;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orckestra.Composer.ApplePay.Api
{
    public class ApplePayController : ApiController
    {
        private readonly ApplePayClient _client;

        public ApplePayController()
        {
            _client = new ApplePayClient();
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

            try
            {
                var request = new ApplePaySessionRequest();
                request.DisplayName = "Pay for Better Retail Order"; // TODO: Have somewhere configured this
                request.Initiative = "web";
                request.InitiativeContext = Request.Headers.Host;

                var merchantSession = await _client.GetMerchantSessionAsync(requestUri, request);

                return Json(merchantSession);
            }
            catch (Exception ex)
            {
                Log.LogError("Appl Pay Controller", ex);
                return Json(ex);
            }
        }
    }
}