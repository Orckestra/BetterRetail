using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Country;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Store.Extentions;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.ViewModels;


namespace Orckestra.Composer.Store.Services
{
    public class StoreDirectoryViewService : IStoreDirectoryViewService
    {
        protected IStoreViewModelFactory StoreViewModelFactory { get; private set; }
        protected IStoreUrlProvider StoreUrlProvider { get; private set; }
        protected IStoreRepository StoreRepository { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }
        protected ICountryService CountryService { get; private set; }

        public StoreDirectoryViewService(
           IStoreViewModelFactory storeViewModelFactory,
           IStoreUrlProvider storeUrlProvider,
           IStoreRepository storeRepository,
           ILocalizationProvider localizationProvider,
           ICountryService countryService)
        {
            StoreViewModelFactory = storeViewModelFactory;
            StoreUrlProvider = storeUrlProvider;
            StoreRepository = storeRepository;
            LocalizationProvider = localizationProvider;
            CountryService = countryService;
        }
        public virtual async Task<StoreDirectoryViewModel> GetStoreDirectoryViewModelAsync(GetStoresParam viewModelParam)
        {
            if (string.IsNullOrWhiteSpace(viewModelParam.Scope)) { throw new ArgumentException("scope"); }
            if (viewModelParam.CultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }
            if (string.IsNullOrWhiteSpace(viewModelParam.BaseUrl)) { throw new ArgumentException("baseUrl"); }

            var model = new StoreDirectoryViewModel();
            model.StoreLocatorPageUrl = StoreUrlProvider.GetStoreLocatorUrl(new GetStoreLocatorUrlParam
            {
                BaseUrl = viewModelParam.BaseUrl,
                CultureInfo = viewModelParam.CultureInfo,
                Page = 1,
                WebsiteId = viewModelParam.WebsiteId
            });

            var overtureStores = await StoreRepository.GetStoresAsync(new GetStoresParam
            {
                CultureInfo = viewModelParam.CultureInfo,
                Scope = viewModelParam.Scope,
                PageNumber = 1,
                PageSize = int.MaxValue
            }).ConfigureAwait(false);

            var sortedResults = SortStoreDirectoryResult(overtureStores.Results);

            //get result for currect page
            var totalCount = sortedResults.Count();
            var result =
                sortedResults.Skip((viewModelParam.PageNumber - 1) * viewModelParam.PageSize)
                    .Take(viewModelParam.PageSize).OrderBy(st => st.Name).ToList();

            var stores =
                result.Select(it => StoreViewModelFactory.CreateStoreViewModel(new CreateStoreViewModelParam
                {
                    Store = it,
                    CultureInfo = viewModelParam.CultureInfo,
                    BaseUrl = viewModelParam.BaseUrl
                })).ToList();

            model.Pagination = BuildPagination(totalCount, viewModelParam);
            model.Groups = GetStoreDirectoryGroups(stores, viewModelParam);
            foreach (var countryGroup in model.Groups)
            {
                countryGroup.Anchors = GetStoreDirectoryCountryGroupAnchors(sortedResults, countryGroup, viewModelParam);
            }

            return model;
        }

        #region Protected Methods
        protected virtual StorePaginationViewModel BuildPagination(int totalCount, GetStoresParam param)
        {
            var totalPages = GetTotalPages(totalCount, param.PageSize);
            StorePageViewModel prevPage = null, nextPage = null;
            if (param.PageSize * param.PageNumber < totalCount)
            {
                nextPage = new StorePageViewModel
                {
                    DisplayName = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
                    {
                        Category = "Store",
                        Key = "B_PaginationNext",
                        CultureInfo = param.CultureInfo
                    }),
                    Url = StoreUrlProvider.GetStoresDirectoryUrl(new GetStoresDirectoryUrlParam
                    {
                        BaseUrl = param.BaseUrl,
                        CultureInfo = param.CultureInfo,
                        Page = param.PageNumber + 1
                    })
                };
            }
            if (param.PageNumber > 1)
            {
                prevPage = new StorePageViewModel
                {
                    DisplayName = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
                    {
                        Category = "Store",
                        Key = "B_PaginationPrev",
                        CultureInfo = param.CultureInfo
                    }),
                    Url = StoreUrlProvider.GetStoresDirectoryUrl(new GetStoresDirectoryUrlParam
                    {
                        BaseUrl = param.BaseUrl,
                        CultureInfo = param.CultureInfo,
                        Page = param.PageNumber - 1
                    })
                };
            }
            var pager = new StorePaginationViewModel
            {
                PreviousPage = prevPage,
                NextPage = nextPage
            };

            return pager;
        }

        protected virtual IList<Overture.ServiceModel.Customers.Stores.Store> SortStoreDirectoryResult(
            IList<Overture.ServiceModel.Customers.Stores.Store> stores)
        {
            return stores
                .Where(st => st.FulfillmentLocation?.Addresses != null && st.FulfillmentLocation.Addresses.Count > 0)
                .OrderBy(st => st.FulfillmentLocation.Addresses[0].CountryCode.ToLower())
                .ThenBy(st => st.FulfillmentLocation.Addresses[0].RegionCode)
                .ThenBy(st => st.FulfillmentLocation.Addresses[0].City).ToList();
        }

        protected virtual IEnumerable<StoreDirectoryGroupViewModel> StoreDirectoryGroupByMany(
            IEnumerable<StoreViewModel> elements,
            params Func<StoreViewModel, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();

                //reduce the list recursively until zero
                var nextSelectors = groupSelectors.Skip(1).ToArray();
                return
                    elements.GroupBy(selector).OrderBy(g => g.Key).Select(
                        g => new StoreDirectoryGroupViewModel
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Items = g.ToList(),
                            SubGroups =
                                nextSelectors.Any() ? StoreDirectoryGroupByMany(g, nextSelectors).ToList() : null
                        });
            }
            return null;
        }

        protected virtual List<StoreDirectoryGroupViewModel> GetStoreDirectoryGroups(IList<StoreViewModel> stores, GetStoresParam viewModelParam)
        {
            var groups = StoreDirectoryGroupByMany(stores, st => st.Address.CountryCode, st => st.Address.RegionCode, st => st.Address.City).ToList();

            foreach (var countryGroup in groups)
            {
                var countryCode = countryGroup.Key.ToString();
                countryGroup.DisplayName = CountryService.RetrieveCountryDisplayNameAsync(new RetrieveCountryParam
                {
                    CultureInfo = viewModelParam.CultureInfo,
                    IsoCode = countryCode
                }).Result ?? countryCode;

                foreach (var regionGroup in countryGroup.SubGroups)
                {
                    regionGroup.DisplayName =
                        CountryService.RetrieveRegionDisplayNameAsync(new RetrieveRegionDisplayNameParam
                        {
                            CultureInfo = viewModelParam.CultureInfo,
                            IsoCode = countryCode,
                            RegionCode = regionGroup.Key.ToString()
                        }).Result;
                }
            }
            return groups;
        }

        protected virtual List<StoreDirectoryAnchorViewModel> GetStoreDirectoryCountryGroupAnchors(IList<Overture.ServiceModel.Customers.Stores.Store> stores,
            StoreDirectoryGroupViewModel countryGroup,
            GetStoresParam viewModelParam)
        {
            var regions = stores.Where(st => st.FulfillmentLocation.Addresses[0].CountryCode == countryGroup.Key.ToString()).Select(st => st.FulfillmentLocation.Addresses[0].RegionCode).Distinct().OrderBy(r => r);
            var anchors = new List<StoreDirectoryAnchorViewModel>();

            foreach (var region in regions)
            {
                var store = stores.First(st => st.FulfillmentLocation.Addresses[0].RegionCode == region);
                var index = stores.IndexOf(store);
                var pageNumber = GetTotalPages(index + 1, viewModelParam.PageSize);
                anchors.Add(new StoreDirectoryAnchorViewModel
                {
                    DisplayName = CountryService.RetrieveRegionDisplayNameAsync(new RetrieveRegionDisplayNameParam
                    {
                        CultureInfo = viewModelParam.CultureInfo,
                        IsoCode = countryGroup.Key.ToString(),
                        RegionCode = region,
                    }).Result,
                    Key = "#" + region,
                    Url = pageNumber == viewModelParam.PageNumber ? string.Empty : StoreUrlProvider.GetStoresDirectoryUrl(new GetStoresDirectoryUrlParam
                    {
                        BaseUrl = viewModelParam.BaseUrl,
                        CultureInfo = viewModelParam.CultureInfo,
                        Page = pageNumber
                    })
                });
            }
            return anchors;
        }
        private int GetTotalPages(int total, int pageSize)
        {
            return (int)Math.Ceiling((double)total / pageSize);
        }
        #endregion
    }
}
