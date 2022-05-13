using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Configuration;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Repositories
{
    public class FulfillmentMethodRepository : IFulfillmentMethodRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public FulfillmentMethodRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

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
            if (string.IsNullOrWhiteSpace(scopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(scopeId)); }

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
