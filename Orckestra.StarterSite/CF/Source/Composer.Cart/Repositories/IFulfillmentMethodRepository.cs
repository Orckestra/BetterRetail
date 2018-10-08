using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Repositories
{
    public interface IFulfillmentMethodRepository
    {
        /// <summary>
        /// Get the Shipping methods available for a shipment.
        /// The Cost and Expected Delivery Date are calculated in overture.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The ShippingMethodsViewModel</returns>
        Task<List<FulfillmentMethod>> GetCalculatedFulfillmentMethods(GetShippingMethodsParam param);
    }
}
