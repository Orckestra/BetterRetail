using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Providers.Facet;
using Orckestra.Composer.Search.Providers.FacetPredicate;
using Orckestra.Composer.Search.Providers.SelectedFacet;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.Services;
using Orckestra.Overture;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.Search
{
    public class SearchPlugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<CategoryBrowsingViewService, ICategoryBrowsingViewService>();
            host.Register<SearchViewService, ISearchViewService>();
            host.Register<SearchRepository, ISearchRepository>();
            host.Register<ProductRequestFactory, IProductRequestFactory>();
            host.Register<SearchRequestContext, ISearchRequestContext>(ComponentLifestyle.PerRequest);
            host.Register<BrowseCategoryRequestContext, IBrowseCategoryRequestContext>(ComponentLifestyle.PerRequest);
            host.Register<SearchBreadcrumbViewService, ISearchBreadcrumbViewService>();
            host.Register<FacetLocalizationProvider, IFacetLocalizationProvider>();
            host.Register<FromPriceProvider, IPriceProvider>();
            host.Register<SearchCategoryRepository, ISearchCategoryRepository>();
            host.Register<SearchManagementRepository, ISearchManagementRepository>();
            host.Register<SearchUrlProvider, ISearchUrlProvider>();

            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(typeof (SearchPlugin).Assembly);

            host.RegisterApiControllers(typeof(SearchPlugin).Assembly);

            RegisterFacetProviders(host);
            RegisterFacetPredicateProviders(host);
            RegisterSelectedFacetProviders(host);
        }

        private static void RegisterFacetPredicateProviders(IComposerHost host)
        {
            host.Register<IFacetPredicateProviderRegistry>(SearchConfiguration.FacetPredicateProviderRegistry);
            host.Register<FacetPredicateFactory, IFacetPredicateFactory>(ComponentLifestyle.Transient);
            host.Register<RangeFacetPredicateProvider>(ComponentLifestyle.Transient);
            host.Register<MultiSelectFacetPredicateProvider>(ComponentLifestyle.Transient);
            host.Register<SingleSelectFacetPredicateProvider>(ComponentLifestyle.Transient);

            SearchConfiguration.FacetPredicateProviderRegistry.RegisterProvider<MultiSelectFacetPredicateProvider>(
                FacetType.MultiSelect.ToString());
            SearchConfiguration.FacetPredicateProviderRegistry.RegisterProvider<RangeFacetPredicateProvider>(
                FacetType.Range.ToString());
            SearchConfiguration.FacetPredicateProviderRegistry.RegisterProvider<SingleSelectFacetPredicateProvider>(
                FacetType.SingleSelect.ToString());
        }

        private static void RegisterFacetProviders(IComposerHost host)
        {
            host.Register<IFacetProviderRegistry>(SearchConfiguration.FacetProviderRegistry);
            host.Register<FacetFactory, IFacetFactory>(ComponentLifestyle.Transient);
            host.Register<RangeFacetProvider>(ComponentLifestyle.Transient);
            host.Register<MultiSelectFacetProvider>(ComponentLifestyle.Transient);
            host.Register<SingleSelectFacetProvider>(ComponentLifestyle.Transient);

            SearchConfiguration.FacetProviderRegistry.RegisterProvider<MultiSelectFacetProvider>(
                FacetType.MultiSelect.ToString());
            SearchConfiguration.FacetProviderRegistry.RegisterProvider<RangeFacetProvider>(FacetType.Range.ToString());
            SearchConfiguration.FacetProviderRegistry.RegisterProvider<SingleSelectFacetProvider>(
                FacetType.SingleSelect.ToString());
        }

        private void RegisterSelectedFacetProviders(IComposerHost host)
        {
            host.Register<ISelectedFacetProviderRegistry>(SearchConfiguration.SelectedFacetProviderRegistry);
            host.Register<SelectedFacetFactory, ISelectedFacetFactory>(ComponentLifestyle.Transient);
            host.Register<RangeSelectedFacetProvider>(ComponentLifestyle.Transient);
            host.Register<MultiSelectSelectedFacetProvider>(ComponentLifestyle.Transient);
            host.Register<SingleSelectSelectedFacetProvider>(ComponentLifestyle.Transient);

            SearchConfiguration.SelectedFacetProviderRegistry.RegisterProvider<MultiSelectSelectedFacetProvider>(
                FacetType.MultiSelect.ToString());
            SearchConfiguration.SelectedFacetProviderRegistry.RegisterProvider<RangeSelectedFacetProvider>(
                FacetType.Range.ToString());
            SearchConfiguration.SelectedFacetProviderRegistry.RegisterProvider<SingleSelectSelectedFacetProvider>(
                FacetType.SingleSelect.ToString());
        }
    }
}