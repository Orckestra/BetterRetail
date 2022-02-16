using System.Web.Http;

namespace Orckestra.Composer.BamboraPayment.Api
{
    public class BamboraController : ApiController
    {
 
        public BamboraController()
        {
       }

        /// <summary>
        /// Get the shopping cart for the current customer
        /// </summary>
        /// <returns>A Json representation of cart state</returns>
        [HttpPost]
        [ActionName("authorize")]
        public IHttpActionResult Authorize()
        {
            return Ok(true);
        }
    }
}