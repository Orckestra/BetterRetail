using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;

namespace Orckestra.Composer.Cart.Repositories.Order
{
    public class RecurringOrdersRepository : IRecurringOrdersRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public RecurringOrdersRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            if (overtureClient == null) { throw new ArgumentNullException(nameof(overtureClient)); }
            if (cacheProvider == null) { throw new ArgumentNullException(nameof(cacheProvider)); }

            OvertureClient = overtureClient;
            CacheProvider = cacheProvider;
        }
        public Task<ListOfRecurringOrderLineItems> GetRecurringOrderTemplates(string scope, Guid customerId)
        {
            if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
            if (customerId == null) { throw new ArgumentNullException(nameof(customerId)); }

            var getRecurringOrderLineItemsForCustomerRequest = new GetRecurringOrderLineItemsForCustomerRequest()
            {
                CustomerId = customerId,
                ScopeId = scope
            };

            return OvertureClient.SendAsync(getRecurringOrderLineItemsForCustomerRequest);
        }

        public Task<RecurringOrderProgram> GetRecurringOrderProgram(string scope, string recurringOrderProgramName)
        {
            if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
            if (recurringOrderProgramName == null) { throw new ArgumentNullException(nameof(recurringOrderProgramName)); }

            var cacheKey = BuildRecurringOrderProgramCacheKey(scope, recurringOrderProgramName);

            var request = new GetRecurringOrderProgramRequest
            {
                RecurringOrderProgramName = recurringOrderProgramName
            };

            return CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        protected CacheKey BuildRecurringOrderProgramCacheKey(string scope, string recurringOrderProgramName)
        {
            var key = new CacheKey(CacheConfigurationCategoryNames.RecurringOrderPrograms)
            {
                Scope = scope
            };

            key.AppendKeyParts(recurringOrderProgramName);
            return key;
        }

    }
}
