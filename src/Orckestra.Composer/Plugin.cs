using System;
using System.Web.Mvc;
using System.Web.Routing;
using Orckestra.Composer.Country;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.RegionCode;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Cookie;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers;
using Orckestra.Composer.ViewEngine.HandleBarsHelpers;
using Orckestra.Overture;

namespace Orckestra.Composer
{
    public class Plugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<ComposerCookieAccessor, ICookieAccessor<ComposerCookieDto>>(ComponentLifestyle.PerRequest);
            host.Register<ComposerContext, IComposerContext>(ComponentLifestyle.PerRequest);
            host.Register<RegionCodeProvider, IRegionCodeProvider>();
            //host.Register<ScopeProvider, IScopeProvider>();
            host.Register<CountryRepository, ICountryRepository>();
            host.Register<CountryService, ICountryService>();
            host.Register<CategoryRepository, ICategoryRepository>();
            host.Register<LookupRepositoryFactory, ILookupRepositoryFactory>();
            host.Register<LookupService, ILookupService>();
            host.Register<AddressRepository, IAddressRepository>();
            host.Register<FulfillmentLocationsRepository, IFulfillmentLocationsRepository>();
            host.Register<ProductSettingsRepository, IProductSettingsRepository>();
            host.Register<ProductSettingsViewService, IProductSettingsViewService>();
            host.Register<ScopeRepository, IScopeRepository>();
            host.Register<ScopeViewService, IScopeViewService>();
            host.Register<InventoryRepository, IInventoryRepository>();
            host.Register<ImageService, IImageService>();
            host.Register<CurrencyConversionSettingsService, ICurrencyConversionSettingsService>();
            host.Register<RecurringOrdersRepository, IRecurringOrdersRepository>();
            host.Register<RecurringOrderTemplatesViewService, IRecurringOrderTemplatesViewService>();
            host.Register<RecurringOrderTemplateViewModelFactory, IRecurringOrderTemplateViewModelFactory>();
            host.Register<RecurringOrderProgramViewModelFactory, IRecurringOrderProgramViewModelFactory>();
            host.Register<ProductPriceViewService, IProductPriceViewService>();
            host.Register<ProductRepository, IProductRepository>();
            host.Register<RegexRulesProvider, IRegexRulesProvider>();

            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(GetType().Assembly);

            host.RegisterControllers(GetType().Assembly);
            host.RegisterApiControllers(GetType().Assembly);

            RegisterSpecializedRoutes(RouteTable.Routes);

            host.Initialized += HostOnInitialized;
        }

        private void HostOnInitialized(object sender, EventArgs eventArgs)
        {
            IComposerHost host = (IComposerHost)sender;

            RegisterHandlebarsHelpers(host);
            RegisterLocalizedHandlebarsHelpers(host);
        }

        /// <summary>
        /// Register routes after initialization so they don't get shallowed by
        /// the default ones
        /// </summary>
        private void RegisterSpecializedRoutes(RouteCollection routeCollection)
        {
            routeCollection.MapRoute("mvc-api.localization", "api/localization/{language}", new
            {
                controller = "Localization",
                action     = "GetTree"
            });
        }

        private void RegisterHandlebarsHelpers(IComposerHost host)
        {
            host.RegisterHandlebarsHelper(new IfEqualsBlockHelper());
            host.RegisterHandlebarsHelper(new IfNotEqualsBlockHelper());
            host.RegisterHandlebarsHelper(new IfExistsBlockHelper());
            host.RegisterHandlebarsHelper(new IfGtBlockHelper());
            host.RegisterHandlebarsHelper(new IfGteBlockHelper());
            host.RegisterHandlebarsHelper(new IfLtBlockHelper());
            host.RegisterHandlebarsHelper(new IfLteBlockHelper());
            host.RegisterHandlebarsHelper(new EscapeHelper());
            host.RegisterHandlebarsHelper(new LookupHelper());
        }

        private void RegisterLocalizedHandlebarsHelpers(IComposerHost host)
        {
            ILocalizationProvider localizationProvider = host.Resolve<ILocalizationProvider>();

            host.RegisterHandlebarsHelper(new FormatValueHelper());
            host.RegisterHandlebarsHelper(new IfIsLocalizedBlockHelper(localizationProvider));
            host.RegisterHandlebarsHelper(new LocalizeHelper(localizationProvider));
            host.RegisterHandlebarsHelper(new LocalizeFormatHelper(localizationProvider));
        }
    }
}
