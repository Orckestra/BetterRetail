using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Repositories
{
    public interface IFulfillmentLocationsRepository
    {
        /// <summary>
        /// Gets the fulfilmment locations for a given scope.
        /// </summary>
        /// <param name="param">Parameters to make request.</param>
        /// <returns></returns>
        Task<List<FulfillmentLocation>> GetFulfillmentLocationsByScopeAsync(GetFulfillmentLocationsByScopeParam param);

        /// <summary>
        /// Get fulfillment location by scope and id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<FulfillmentLocation> GetFulfillmentLocationByIdAsync(Guid id, string scope);
    }
}
