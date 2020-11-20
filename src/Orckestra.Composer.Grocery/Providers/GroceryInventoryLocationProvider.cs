using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Providers
{
    public class GroceryInventoryLocationProvider : ConfigurationInventoryLocationProvider
    {
        public GroceryInventoryLocationProvider(
            IFulfillmentLocationsRepository fulfillmentLocationsRepository,
            IInventoryRepository inventoryRepository,
            IWebsiteContext websiteContext,
            ISiteConfiguration siteConfiguration,
            IStoreAndFulfillmentSelectionProvider storeAndFulfillmentSelectionProvider,
            IComposerContext composerContext)
            : base(fulfillmentLocationsRepository, inventoryRepository, websiteContext, siteConfiguration)
        {
            StoreAndFulfillmentSelectionProvider = storeAndFulfillmentSelectionProvider ?? throw new ArgumentNullException(nameof(storeAndFulfillmentSelectionProvider));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        internal IComposerContext ComposerContext { get; }
        internal IStoreAndFulfillmentSelectionProvider StoreAndFulfillmentSelectionProvider { get; }
        public override async Task<string> GetDefaultInventoryLocationIdAsync()
        {
            var store = await StoreAndFulfillmentSelectionProvider.GetSelectedStoreAsync(new GetSelectedFulfillmentParam
            {
                TryGetFromDefaultSettings = true,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo
            });

            return await Task.FromResult(store?.FulfillmentLocation?.InventoryLocationId
                ?? SiteConfiguration.GetInventoryAndFulfillmentLocationId(WebsiteContext.WebsiteId)).ConfigureAwait(false);
        }

        public override async Task<List<string>> GetInventoryLocationIdsForSearchAsync()
        {
            var fulfillmentLocation = await GetDefaultInventoryLocationIdAsync().ConfigureAwait(false);
            return new List<string>() { fulfillmentLocation };
        }

        protected override GetFulfillmentLocationsByScopeParam GetFulfillmentLocationsByScopeParam(GetFulfillmentLocationParam param)
        {
            var newParam = base.GetFulfillmentLocationsByScopeParam(param);
            newParam.IncludeChildScopes = true;
            return newParam;
        }
    }
}