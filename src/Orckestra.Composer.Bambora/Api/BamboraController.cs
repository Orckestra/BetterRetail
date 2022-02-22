using Composite.Core;
using Orckestra.Composer.BamboraPayment.Parameters;
using System;
using System.Web.Http;

namespace Orckestra.Composer.BamboraPayment.Api
{
    public class BamboraController : ApiController
    {

        protected BamboraGateway _client;
        public BamboraController()
        {
            try
            {
                _client = new BamboraGateway();
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

            var response = _client.PreAuth(param.Amount, param.Token);

            return Ok(response);
        }
    }
}