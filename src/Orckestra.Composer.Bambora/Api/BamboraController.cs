using System;
using System.Web.Http;

namespace Orckestra.Composer.BamboraPayment.Api
{
    public class BamboraController : ApiController
    {

        protected BamboraGateway _client;
        public BamboraController()
        {
            _client = new BamboraGateway();
       }

        /// <summary>
        /// Get the shopping cart for the current customer
        /// </summary>
        /// <returns>A Json representation of cart state</returns>
        [HttpPost]
        [ActionName("authorize")]
        public IHttpActionResult Authorize([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            var response = _client.PreAuth(1, "1000", token);

            return Ok(response);
        }
    }
}