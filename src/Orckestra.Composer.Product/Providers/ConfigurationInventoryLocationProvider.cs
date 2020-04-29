using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Product.Providers
{
    public class ConfigurationInventoryLocationProvider : IInventoryLocationProvider
    {
        public IFulfillmentLocationsRepository FulfillmentLocationsRepository { get; set; }
        public IInventoryRepository InventoryRepository { get; set; }
        public IWebsiteContext WebsiteContext { get; set; }
        public ISiteConfiguration SiteConfiguration { get; set; }

        public ConfigurationInventoryLocationProvider(
            IFulfillmentLocationsRepository fulfillmentLocationsRepository, 
            IInventoryRepository inventoryRepository,
            IWebsiteContext websiteContext,
            ISiteConfiguration siteConfiguration)
        {
            FulfillmentLocationsRepository = fulfillmentLocationsRepository ?? throw new ArgumentNullException(nameof(fulfillmentLocationsRepository));
            InventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
            SiteConfiguration = siteConfiguration;
        }

        /// <summary>
        /// Business id for the inventory location which will be associated to the Sku to retrieve InventoryItemStatus
        /// </summary>
        /// <returns></returns>
        public virtual Task<string> GetDefaultInventoryLocationIdAsync()
        {
            return Task.FromResult(SiteConfiguration.GetInventoryAndFulfillmentLocationId(WebsiteContext.WebsiteId));
        }

        /// <summary>
        /// Business id for the inventory locations which will be used by product searches
        /// </summary>
        /// <returns></returns>
        public virtual Task<List<string>> GetInventoryLocationIdsForSearchAsync()
        {
            var task = Task.FromResult(new List<string> { SiteConfiguration.GetInventoryAndFulfillmentLocationId(WebsiteContext.WebsiteId) });
            task.ConfigureAwait(false);
            return task;
        }

        public virtual string SetDefaultInventoryLocationId(string inventoryLocationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtains the fulfillment location to use for a cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<FulfillmentLocation> GetFulfillmentLocationAsync(GetFulfillmentLocationParam param)
        {
            var p = GetFulfillmentLocationsByScopeParam(param);

            var getLocationsTask = FulfillmentLocationsRepository.GetFulfillmentLocationsByScopeAsync(p);
            //TODO: See Bug #6064 - The search crash when inventory is disabled and the fulfillment location is wrong
            var getDefaultLocationIdTask = GetDefaultInventoryLocationIdAsync();
            await Task.WhenAll(getLocationsTask, getDefaultLocationIdTask).ConfigureAwait(false);

            var locations = await getLocationsTask;
            var defaultLocationId = await getDefaultLocationIdTask;

            var location = GetMatchingLocation(locations, defaultLocationId);
            if (location == null)
            {
                throw new InvalidOperationException($"Could not find any active fulfillment location in the scope '{param.Scope}' " +
                    $"to support the Inventory Location Id '{defaultLocationId}'");
            }

            return location;
        }

        protected virtual GetFulfillmentLocationsByScopeParam GetFulfillmentLocationsByScopeParam(GetFulfillmentLocationParam param)
        {
            var p = new GetFulfillmentLocationsByScopeParam
            {
                Scope = param.Scope,
                IncludeChildScopes = false,
                IncludeSchedules = false
            };

            return p;
        }

        protected virtual FulfillmentLocation GetFirstValidLocation(IEnumerable<FulfillmentLocation> fulfillmentLocations)
        {
            var location = fulfillmentLocations.FirstOrDefault(loc => loc.IsActive);
            return location;
        }

        protected virtual FulfillmentLocation GetMatchingLocation(IEnumerable<FulfillmentLocation> fulfillmentLocations, string locationId)
        {
            var location =
                fulfillmentLocations.FirstOrDefault(
                    loc =>
                        loc.IsActive &&
                        string.Equals(loc.InventoryLocationId, locationId, StringComparison.InvariantCultureIgnoreCase));

            return location;
        }
    }
}