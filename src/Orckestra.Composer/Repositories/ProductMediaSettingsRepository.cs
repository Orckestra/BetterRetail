using System;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Requests.Products;

namespace Orckestra.Composer.Repositories
{
    public class ProductMediaSettingsRepository: IProductMediaSettingsRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public ProductMediaSettingsRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Retrieve the Product Media Settings from Overture
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual async Task<MediaSettings> GetProductMediaSettings()
        {
            var productMediaSettingsCacheKey = new CacheKey(CacheConfigurationCategoryNames.ProductSettings);
            productMediaSettingsCacheKey.AppendKeyParts("Media");

            return await CacheProvider.GetOrAddAsync(productMediaSettingsCacheKey, () => 
                OvertureClient.SendAsync(new GetMediaSettingsRequest())
            ).ConfigureAwait(false);
        }
    }
}
