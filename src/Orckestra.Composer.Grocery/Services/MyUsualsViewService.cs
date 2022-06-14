using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Grocery.Settings;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Orckestra.Composer.Grocery.Services
{
    public class MyUsualsViewService : IMyUsualsViewService
    {
        private IMyUsualsSettings MyUsualsSettings { get; }
        protected IOrderRepository OrderRepository { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }
        protected IBaseSearchCriteriaProvider BaseSearchCriteriaProvider { get; private set; }
        protected HttpRequestBase Request { get; private set; }


        public MyUsualsViewService(
            IMyUsualsSettings myUsualsSettings,
            IOrderRepository orderRepository,
            IBaseSearchCriteriaProvider baseSearchCriteriaProvider,
            HttpRequestBase request,
            ISearchUrlProvider searchUrlProvider)
        {
            MyUsualsSettings = myUsualsSettings ?? throw new ArgumentNullException(nameof(myUsualsSettings));
            OrderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            BaseSearchCriteriaProvider = baseSearchCriteriaProvider ?? throw new ArgumentNullException(nameof(baseSearchCriteriaProvider));
            Request = request;
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));

        }

        public virtual async Task<string[]> GetMyUsualsProductSkusAsync(GetCustomerOrderedProductsParam param)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-MyUsualsSettings.TimeFrame);

            var response = await OrderRepository.GetCustomerOrderedProductsAsync(new GetCustomerOrderedProductsParam
            {
                ScopeId = param.ScopeId,
                CustomerId = param.CustomerId,
                StartDate = startDate,
                EndDate = endDate,
                MinimumOrderedNumberOfTimes = MyUsualsSettings.Frequency,
                MaximumItems = GroceryConfiguration.MaxOrderedProductsItems
            }).ConfigureAwait(false);

            var listSkus = response.OrderedProducts.Select(item => item.Sku).ToArray();

            return listSkus;
        }

        public virtual async Task<SearchBySkusCriteria> BuildProductsSearchCriteria(string[] listSkus, string QueryString, bool IncludeFactes = true)
        {
            var queryString = HttpUtility.ParseQueryString(QueryString ?? "");
            var SelectedFacets = SearchUrlProvider.BuildSelectedFacets(queryString).ToList();
            var Keywords = queryString[SearchRequestParams.Keywords] ?? "*";
            var CurrentPage = int.TryParse(queryString[SearchRequestParams.Page], out int page) && page > 0 ? page : 1;
            var SortDirection = queryString[SearchRequestParams.SortDirection] ?? SearchRequestParams.DefaultSortDirection;
            var SortBy = queryString[SearchRequestParams.SortBy] ?? SearchRequestParams.DefaultSortBy;
            var BaseUrl = RequestUtils.GetBaseUrl(Request).ToString();

            var searchCriteria = await BaseSearchCriteriaProvider.GetSearchCriteriaAsync(Keywords, BaseUrl, IncludeFactes, CurrentPage).ConfigureAwait(false);
            var searchBySkusCriteria = new SearchBySkusCriteria
            {
                Skus = listSkus,
                Keywords = searchCriteria.Keywords,
                NumberOfItemsPerPage = searchCriteria.NumberOfItemsPerPage,
                StartingIndex = searchCriteria.StartingIndex,
                Page = searchCriteria.Page,
                BaseUrl = searchCriteria.BaseUrl,
                Scope = searchCriteria.Scope,
                CultureInfo = searchCriteria.CultureInfo,
                InventoryLocationIds = searchCriteria.InventoryLocationIds,
                AvailabilityDate = searchCriteria.AvailabilityDate,
                IncludeFacets = searchCriteria.IncludeFacets
            };
            searchBySkusCriteria.SortBy = SortBy;
            searchBySkusCriteria.SortDirection = SortDirection;
            searchBySkusCriteria.SelectedFacets.AddRange(SelectedFacets);

            return searchBySkusCriteria;
        }
    }
}
