using System;
using System.Threading.Tasks;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Requests.Metadata;

namespace Orckestra.Composer.CustomerConditionProvider.Repositories
{
    public class CustomerDefinitionsRepository : ICustomerDefinitionsRepository
    {
        protected IOvertureClient OvertureClient { get; }
        protected ICacheProvider CacheProvider { get; }

        public CustomerDefinitionsRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }
        public virtual async Task<EntityDefinition> GetCustomerDefinitionAsync()
        {
            var definition = await OvertureClient.SendAsync(new GetCustomerDefinitionRequest()
            {
                Name = "CUSTOMER"
            }).ConfigureAwait(false);

            return definition;
        }
    }
}
