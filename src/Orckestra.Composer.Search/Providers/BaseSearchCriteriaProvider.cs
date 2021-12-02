using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Services;
using System;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Providers
{
    public class BaseSearchCriteriaProvider : IBaseSearchCriteriaProvider
    {
        protected IWebsiteContext WebsiteContext { get; private set; }
        protected IInventoryLocationProvider InventoryLocationProvider { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected IFulfillmentContext FulfillmentContext { get; private set; }

        public BaseSearchCriteriaProvider(IWebsiteContext websiteContext, IInventoryLocationProvider inventoryLocationProvider, IComposerContext composerContext, IFulfillmentContext fulfillmentContext)
        {
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext)); ;
            InventoryLocationProvider = inventoryLocationProvider ?? throw new ArgumentNullException(nameof(inventoryLocationProvider));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            FulfillmentContext = fulfillmentContext ?? throw new ArgumentNullException(nameof(fulfillmentContext));
        }

        public virtual async Task<SearchCriteria> GetSearchCriteriaAsync(string searchTerms, string baseURL, bool includeFacets, int page)
        {
            return new SearchCriteria
            {
                Keywords = searchTerms,
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                IncludeFacets = includeFacets,
                StartingIndex = (page - 1) * SearchConfiguration.MaxItemsPerPage,
                Page = page,
                SortBy = SearchRequestParams.DefaultSortBy,
                SortDirection = SearchRequestParams.DefaultSortDirection,
                BaseUrl = baseURL,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                InventoryLocationIds = await InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().ConfigureAwait(false),
                AvailabilityDate = FulfillmentContext.AvailabilityAndPriceDate,
                AutoCorrect = SearchConfiguration.AutoCorrectSearchTerms,
            };
        }
    }
}
