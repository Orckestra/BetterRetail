using System.Web;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.CompositeC1.Controllers.Helpers
{
    public static class SearchControllerHelper
    {
        public static SearchCriteria GetSearchCriteria(HttpRequestBase request, IInventoryLocationProvider inventoryLocationProvider,  
            IComposerContext composerContext, string keywords, int page, string sortBy, string sortDirection)
        {
            return new SearchCriteria
            {
                Keywords = keywords,
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                IncludeFacets = true,
                StartingIndex = (page - 1) * SearchConfiguration.MaxItemsPerPage,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                BaseUrl = RequestUtils.GetBaseUrl(request).ToString(),
                CultureInfo = composerContext.CultureInfo,
                Scope = composerContext.Scope,
                InventoryLocationIds = inventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().Result,
                AutoCorrect = SearchConfiguration.AutoCorrectSearchTerms,
            };
        }

        public static bool AreKeywordsValid(string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                return false;
            }

            var strippedKeywords = keywords.Trim();

            if (SearchConfiguration.BlockStarSearchs)
            {
                strippedKeywords = strippedKeywords.Replace("*", "");
            }

            var isInvalid = string.IsNullOrWhiteSpace(strippedKeywords);
            return !isInvalid;
        }


    };
}