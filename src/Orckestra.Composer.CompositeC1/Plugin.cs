using System;
using Orckestra.Composer.CompositeC1.Mappers;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.CompositeC1.Providers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.CompositeC1.Services.DataQuery;
using Orckestra.Composer.CompositeC1.Services.Facet;
using Orckestra.Composer.HttpModules;
using Orckestra.Composer.Mvc.Sample.Providers.UrlProvider;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Overture;
using Orckestra.Composer.CompositeC1.Settings;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Search.Context;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.Settings;
using Orckestra.Composer.CompositeC1.Services.PreviewMode;
using Orckestra.Composer.CompositeC1.Providers.Breadcrumb;
using Orckestra.Composer.CompositeC1.Providers.LanguageSwitch;

namespace Orckestra.Composer.CompositeC1
{
    public class Plugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            RegisterDependencies(host);

            CategoriesConfiguration.CategoriesSyncConfiguration.Add("RootPageId", new Guid("f3dbd28d-365f-4d3e-91c3-7b730b39b294"));
        }

        private void RegisterDependencies(IComposerHost host)
        {
            host.Register<RecurringOrdersSettings, IRecurringOrdersSettings>(ComponentLifestyle.PerRequest);
            host.Register<GoogleSettings, IGoogleSettings>(ComponentLifestyle.PerRequest);
            host.Register<WebsiteContext, IWebsiteContext>(ComponentLifestyle.PerRequest);
            host.Register<EmptyFulfillmentContext, IFulfillmentContext>(ComponentLifestyle.Singleton);
            host.Register<AntiCookieTamperingExcluder, IAntiCookieTamperingExcluder>();
            host.Register<Providers.ScopeProvider, IScopeProvider>(ComponentLifestyle.PerRequest);
            host.Register<Providers.ProductUrlProvider, IProductUrlProvider>(ComponentLifestyle.PerRequest);
            host.Register<Providers.CountryCodeProvider, ICountryCodeProvider>(ComponentLifestyle.PerRequest);
            host.Register<FacetConfigurationContext, IFacetConfigurationContext>(ComponentLifestyle.PerRequest);
            host.Register<CategoryMetaContext, ICategoryMetaContext>(ComponentLifestyle.PerRequest);
            host.Register<PageService, IPageService>();
            host.Register<CultureService, ICultureService>(ComponentLifestyle.Singleton);
            host.Register<GoogleAnalyticsNavigationUrlProvider>();
            host.Register<NavigationMapper, INavigationMapper>();

            host.Register<SearchUrlProvider, ISearchUrlProvider>();
            host.Register<CategoryBrowsingUrlProvider, ICategoryBrowsingUrlProvider>();
            host.Register<CartUrlProvider, ICartUrlProvider>();
            host.Register<MyAccountUrlProvider, IMyAccountUrlProvider>();
            host.Register<WishListUrlProvider, IWishListUrlProvider>();
            host.Register<RecurringScheduleUrlProvider, IRecurringScheduleUrlProvider>();
            host.Register<RecurringCartUrlProvider, IRecurringCartUrlProvider>();
            host.Register<CategoryPageService, ICategoryBrowsingService>();
            host.Register<ImageViewService, IImageViewService>();
            host.Register<MediaService, IMediaService>();
            host.Register<OrderUrlProvider, IOrderUrlProvider>();
            host.Register<Providers.StoreUrlProvider, IStoreUrlProvider>();
            host.Register<LanguageSwitchViewService, ILanguageSwitchService>();
            host.Register<BreadcrumbViewService, IBreadcrumbViewService>();
            // TODO: Why not done in Composer directly ??
            host.Register<SettingsFromConfigFileService, ISettingsService>();
            host.Register<MyAccountViewService, IMyAccountViewService>();
            host.Register<PageNotFoundUrlProvider, IPageNotFoundUrlProvider>();
            host.Register<C1PerformanceDataCollector, IPerformanceDataCollector>();
            host.Register<DataQueryService, IDataQueryService>();
            host.Register<SiteConfiguration, ISiteConfiguration>(ComponentLifestyle.Singleton);
            host.Register<CookieAccesserSettings, ICookieAccesserSettings>(ComponentLifestyle.Singleton);
            host.Register<CdnDamProviderSettings, ICdnDamProviderSettings>(ComponentLifestyle.Singleton);
            host.Register<CacheService, ICacheService>(ComponentLifestyle.Singleton);
            host.Register<PreviewModeService, IPreviewModeService>();
            host.Register<AutocompleteProvider, IAutocompleteProvider>();
            host.Register<Scheduler, IScheduler>(ComponentLifestyle.Singleton);

            host.Register<ProductContext, IProductContext>(ComponentLifestyle.PerRequest);
            host.Register<StoreContext, IStoreContext>(ComponentLifestyle.PerRequest);
            host.Register<LazyFunctionCallDataProvider, ILazyFunctionCallDataProvider>();
            host.Register<BaseSearchCriteriaProvider, IBaseSearchCriteriaProvider>();
            RegisterBreadcrumProviders(host);
            RegisterLanguageSwitchProviders(host);

            host.RegisterApiControllers(typeof(Plugin).Assembly);
        }

        private static void RegisterBreadcrumProviders(IComposerHost host)
        {
            host.Register<BreadcrumbContext, IBreadcrumbContext>(ComponentLifestyle.PerRequest);
            host.Register<BreadcrumbProvider, IBreadcrumbProvider>();
            host.Register<ProductBreadcrumbProvider, IBreadcrumbProvider>();
            host.Register<StoreBreadcrumbProvider, IBreadcrumbProvider>();
            host.Register<ChekoutBreadcrumbProvider, IBreadcrumbProvider>();
            host.Register<SearchBreadcrumbProvider, IBreadcrumbProvider>();
        }

        private static void RegisterLanguageSwitchProviders(IComposerHost host)
        {
            host.Register<LanguageSwitchContext, ILanguageSwitchContext>(ComponentLifestyle.PerRequest);
            host.Register<LanguageSwitchProvider, ILanguageSwitchProvider>();
            host.Register<ProductLanguageSwitchProvider, ILanguageSwitchProvider>();
            host.Register<StoreLanguageSwitchProvider, ILanguageSwitchProvider>();
        }
    }
}
