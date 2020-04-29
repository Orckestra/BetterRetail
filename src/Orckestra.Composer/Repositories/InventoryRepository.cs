using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using Orckestra.Overture.ServiceModel.Requests.Products.Inventory;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;


namespace Orckestra.Composer.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public InventoryRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Retrieve the detail about the status of Inventory Items represented by the specified InventoryLocationId and a list of skus for the specified date
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<List<InventoryItemAvailability>> FindInventoryItemStatus(FindInventoryItemStatusParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.Skus == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Skus)), nameof(param)); }
            if (param.Skus.Count == 0) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.Skus)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.InventoryLocationId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.InventoryLocationId)), nameof(param)); }

            var request = new FindInventoryItemStatusByLocationAndSkusRequest
            {
                Date = param.Date,
                InventoryLocationId = param.InventoryLocationId,
                ScopeId = param.Scope,
                Skus = param.Skus
            };

            var result = await OvertureClient.SendAsync(request).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Retrieve a list of InventoryItemStatusDetails represented by sku from all inventory location associated to the specific scope. The list of inventory location ids is all existing Fulfillment Locations inside the specific Scope
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<InventoryItemStatusDetailsQueryResult> GetInventoryItemsBySkuAsync(GetInventoryItemsBySkuParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.Sku == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Sku)), nameof(param)); }

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.StoreInventoryItems)
            {
                Scope = param.Scope
            };
            cacheKey.AppendKeyParts("sku", param.Sku);

            var request = new GetInventoryItemsByScopeAndSkuRequest
            {
                Date = param.Date,
                Sku = param.Sku,
                ScopeId = param.Scope,
                IncludeChildScopes = param.IncludeChildScopes
            };

            return await CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request)).ConfigureAwait(false);
        }
    }
}
