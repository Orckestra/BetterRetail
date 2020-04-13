using System;
using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Store.Extentions;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Composer.Utils;
using System.Web;
using Orckestra.Composer.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.Store.Services
{
    public class StoreViewService : IStoreViewService
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        protected IStoreViewModelFactory StoreViewModelFactory { get; private set; }
        protected IStoreRepository StoreRepository { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }
        protected IStoreUrlProvider StoreUrlProvider { get; private set; }

        public StoreViewService(
            IStoreRepository storeRepository,
            IStoreViewModelFactory storeViewModelFactory,
            ILocalizationProvider localizationProvider,
            IStoreUrlProvider storeUrlProvider)
        {
            StoreViewModelFactory = storeViewModelFactory;
            StoreRepository = storeRepository;
            LocalizationProvider = localizationProvider;
            StoreUrlProvider = storeUrlProvider;
        }

        public virtual async Task<StoreViewModel> GetStoreViewModelAsync(GetStoreByNumberParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentNullException("scope"); }
            if (param.CultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }
            if (string.IsNullOrWhiteSpace(param.StoreNumber)) { throw new ArgumentNullException("storeNumber"); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentNullException("baseUrl"); }

            var overtureStore = await StoreRepository.GetStoreByNumberAsync(param).ConfigureAwait(false);
            if (overtureStore == null)
            {
                return null;
            }

            var createVmParam = new CreateStoreViewModelParam
            {
                Store = overtureStore,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl,
                SearchPoint = param.SearchPoint
            };
            var storeViewModel = StoreViewModelFactory.CreateStoreViewModel(createVmParam);

            storeViewModel.StructuredData = StoreViewModelFactory.CreateStoreStructuredDataViewModel(createVmParam);

            if (storeViewModel.HasLocation())
            {
                storeViewModel.Context.Add("latitude", storeViewModel.GetLatitude().ToString(CultureInfo.InvariantCulture));
                storeViewModel.Context.Add("longitude", storeViewModel.GetLongitude().ToString(CultureInfo.InvariantCulture));
            }

            return storeViewModel;
        }

        public virtual async Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(
            GetStorePageHeaderViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException("storepageheaderparam");}

            var store = await GetStoreViewModelAsync(new GetStoreByNumberParam
            {
                StoreNumber = param.StoreNumber,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope,
                BaseUrl = param.BaseUrl

            }).ConfigureAwait(false);

            if (store == null)
            {
                return null;
            }

            var vm = new PageHeaderViewModel
            {
                PageTitle = GetStorePageTitle(store),
                CanonicalUrl = GetStoreCanonicalUrl(param, store),
                MetaDescription = GetStoreMetaDescription(param, store)
            };

            return vm;
        }

        protected virtual string GetStorePageTitle(StoreViewModel store)
        {
            return store.LocalizedDisplayName;
        }

        protected virtual string GetStoreCanonicalUrl(GetStorePageHeaderViewModelParam param, StoreViewModel store)
        {
            var relativeUri = StoreUrlProvider.GetStoreUrl(new GetStoreUrlParam
            {
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo,
                StoreNumber = param.StoreNumber,
                StoreName = store.Name
            });

            if (HttpContext.Current == null)
            {
                Log.Error("HttpContext.Current is null");

                return relativeUri;
            }

            try
            {
                var baseUri = RequestUtils.GetBaseUrl(HttpContext.Current.Request.Url);

                var url = new Uri(baseUri, relativeUri);

                return url.ToString();
            }
            catch (Exception ex)
            {
                string fullStackTrace = ex.StackTrace + Environment.StackTrace;
                Log.Error($"Error retrieving store canonical url. Exception : {ex}, {fullStackTrace}");

                return relativeUri;
            }
        }

        protected virtual string GetStoreMetaDescription(GetStorePageHeaderViewModelParam param, StoreViewModel store)
        {
            var template = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "Store",
                CultureInfo = param.CultureInfo,
                Key = "M_Description"
            });
            if (!string.IsNullOrWhiteSpace(template) && template.Contains("{0}"))
            {
                return string.Format(template, store.LocalizedDisplayName);
            }
            return template;
        }

        public virtual async Task<List<StoreViewModel>> GetStoresForInStorePickupViewModelAsync(GetStoresForInStorePickupViewModelParam param)
        {
            var getStoresParam = new GetStoresParam
            {
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope
            };

            var stores = await StoreRepository.GetStoresAsync(getStoresParam).ConfigureAwait(false);
            if (stores.Results != null)
            {
                var vm = stores.Results
                                .Where(s => s.IsActive && s.FulfillmentLocation != null && s.FulfillmentLocation.IsPickUpLocation)
                                .Select(s => StoreViewModelFactory.CreateStoreViewModel(new CreateStoreViewModelParam
                                {
                                    BaseUrl = param.BaseUrl,
                                    CultureInfo = param.CultureInfo,
                                    Store = s
                                })).ToList();

                return vm;
            }

            return new List<StoreViewModel>();
        }
    }
}
