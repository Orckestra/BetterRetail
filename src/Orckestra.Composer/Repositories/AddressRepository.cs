using System;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests.Customers.Addresses;

namespace Orckestra.Composer.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public AddressRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Gets a single Address based on it's unique identifier
        /// </summary>
        /// <param name="addressId">The unique Id for the address to find</param>
        /// <returns>
        /// The Address matching the requested ID, or null
        /// </returns>
        public Task<Address> GetAddressByIdAsync(Guid addressId)
        {
            if (addressId == Guid.Empty) { throw new ArgumentNullException(nameof(addressId)); }

            var request = new GetAddressRequest
            {
                AddressId = addressId
            };

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Address);
            cacheKey.AppendKeyParts(addressId);

            return CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }
    }
}
