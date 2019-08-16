using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders;

namespace Orckestra.Composer.Cart.Repositories
{
    public class FulfillmentMethodRepository : IFulfillmentMethodRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public FulfillmentMethodRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            if (overtureClient == null) { throw new ArgumentNullException("overtureClient"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            OvertureClient = overtureClient;
            CacheProvider = cacheProvider;
        }

        /// <summary>
        /// Get the Shipping methods available for a shipment.
        /// The Cost and Expected Delivery Date are calculated in overture.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The ShippingMethodsViewModel</returns>
        /// <remarks>This cannot be cached because Overture may include custom logic depending on the current cart state.</remarks>
        public virtual Task<List<FulfillmentMethod>> GetCalculatedFulfillmentMethods(GetShippingMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), "param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), "param"); }

            var request = new FindCalculatedFulfillmentMethodsRequest
            {
                CartName = param.CartName,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                ShipmentId = param.ShipmentId
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual Task<GetFulfillmentMethodsResponse> GetFulfillmentMethods(string scopeId)
        {            
            if (string.IsNullOrWhiteSpace(scopeId)) { throw new ArgumentNullException("scopeId", "scopeId is required"); }

            var cacheKey = GetCacheKeyForFulfillmentMethodsByScope(scopeId);

            var request = new GetAvailableFulfillmentMethodsByScopeRequest { ScopeId = scopeId };

            return CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }
        protected static CacheKey GetCacheKeyForFulfillmentMethodsByScope(string scopeId)
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.FulfillmentMethodsByScope, scopeId);

            return cacheKey;
        }
    }
}
