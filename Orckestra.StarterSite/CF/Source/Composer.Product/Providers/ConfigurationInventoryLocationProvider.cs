using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Product.Providers
{
    public class ConfigurationInventoryLocationProvider : IInventoryLocationProvider
    {
        public IFulfillmentLocationsRepository FulfillmentLocationsRepository { get; set; }
        public IInventoryRepository InventoryRepository { get; set; }
        public IWebsiteContext WebsiteContext { get; set; }

        public ConfigurationInventoryLocationProvider(
            IFulfillmentLocationsRepository fulfillmentLocationsRepository, 
            IInventoryRepository inventoryRepository,
            IWebsiteContext websiteContext)
        {
            if (fulfillmentLocationsRepository == null) { throw new ArgumentNullException("fulfillmentLocationsRepository"); }
            if (inventoryRepository == null) { throw new ArgumentNullException("inventoryRepository"); }
            if (websiteContext == null) { throw new ArgumentNullException(nameof(websiteContext)); }

            FulfillmentLocationsRepository = fulfillmentLocationsRepository;
            InventoryRepository = inventoryRepository;
            WebsiteContext = websiteContext;
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
            return Task.FromResult(new List<string> { SiteConfiguration.GetInventoryAndFulfillmentLocationId(WebsiteContext.WebsiteId) });
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
            var defaultLocationIdTask = GetDefaultInventoryLocationIdAsync();
            await Task.WhenAll(getLocationsTask, defaultLocationIdTask).ConfigureAwait(false);

            var location = GetMatchingLocation(getLocationsTask.Result, defaultLocationIdTask.Result);
            if (location == null)
            {
                throw new ArgumentException(String.Format("Could not find any active fulfillment location in the scope '{0}' to support the Inventory Location Id '{1}'",
                    param.Scope, defaultLocationIdTask.Result), "param");
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
                        String.Equals(loc.InventoryLocationId, locationId, StringComparison.InvariantCultureIgnoreCase));

            return location;
        }
    }
}
