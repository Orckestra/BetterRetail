using Composite.Core;
using Orckestra.Composer.BamboraPayment.Parameters;
using System;
using System.Web.Http;

namespace Orckestra.Composer.BamboraPayment.Api
{
    public class BamboraController : ApiController
    {

        protected BamboraApplePayGateway _client;
        public BamboraController()
        {
            try
            {
                _client = new BamboraApplePayGateway();
            }
            catch (Exception ex)
            {
                Log.LogError("BamboraGateway", ex);
                throw;
            }
        }

        /// <summary>
        /// Get the shopping cart for the current customer
        /// </summary>
        /// <returns>A Json representation of cart state</returns>
        [HttpPost]
        [ActionName("authorize")]
        public IHttpActionResult Authorize([FromBody] AuthorizePaymentParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrEmpty(param.Token)) throw new ArgumentNullException(nameof(param.Token));
            // in real Payment Provider implemantation need to add also Billing and other things
            var response = _client.PreAuth(param.Amount, param.Token);

            return Ok(response);
        }
    }
}