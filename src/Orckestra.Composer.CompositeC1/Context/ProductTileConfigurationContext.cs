using Composite.Data;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.CompositeC1.Services.DataQuery;
using System.Collections.Generic;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Context
{
    public class ProductTileConfigurationContext : IProductTileConfigurationContext
    {
        protected ICacheStore<string, List<IPromotionalRibbonConfiguration>> PromotionalRibbonCache { get; }
        protected ICacheStore<string, List<IPromotionalBannerConfiguration>> PromotionalBannerCache { get; }
        protected HttpContextBase HttpContext { get; }
        protected IDataQueryService DataQueryService { get; }
        private List<IPromotionalRibbonConfiguration> _promotionalRibbonConfigurations;
        public string PromotionalRibbonDefaultBackgroundColor => "bg-dark";
        public string PromotionalRibbonDefaultTextColor => "text-white";
        private List<IPromotionalBannerConfiguration> _promotionalBannerConfigurations;
        public string PromotionalBannerDefaultBackgroundColor => "bg-dark";
        public string PromotionalBannerDefaultTextColor => "text-white";

        public ProductTileConfigurationContext(HttpContextBase httpContext, IDataQueryService dataQueryService, ICacheService cacheService)
        {
            HttpContext = httpContext;
            DataQueryService = dataQueryService;
            PromotionalRibbonCache = cacheService.GetStoreWithDependencies<string, List<IPromotionalRibbonConfiguration>>("Promotional Ribbons",
                new CacheDependentEntry<IPromotionalRibbonConfiguration>());

            PromotionalBannerCache = cacheService.GetStoreWithDependencies<string, List<IPromotionalBannerConfiguration>>("Promotional Banners",
                new CacheDependentEntry<IPromotionalBannerConfiguration>());
        }
        public List<IPromotionalRibbonConfiguration> GetPromotionalRibbonConfigurations()
        {
            if (_promotionalRibbonConfigurations == null)
            {
                _promotionalRibbonConfigurations = PromotionalRibbonCache.GetOrAdd("PromotionalRibbonSettings", _ => LoadPromotionalRibbonSettings());
            }

            return _promotionalRibbonConfigurations;
        }

        private List<IPromotionalRibbonConfiguration> LoadPromotionalRibbonSettings()
        {
            var promotionalRibbons = new List<IPromotionalRibbonConfiguration>();
            using (var con = new DataConnection())
            {
                promotionalRibbons.AddRange(con.Get<IPromotionalRibbonConfiguration>());
            }

            return promotionalRibbons;
        }

        public List<IPromotionalBannerConfiguration> GetPromotionalBannerConfigurations()
        {
            if (_promotionalBannerConfigurations == null)
            {
                _promotionalBannerConfigurations = PromotionalBannerCache.GetOrAdd("PromotionalBannerSettings", _ => LoadPromotionalBannerSettings());
            }

            return _promotionalBannerConfigurations;
        }

        private List<IPromotionalBannerConfiguration> LoadPromotionalBannerSettings()
        {
            var promotionalBanners = new List<IPromotionalBannerConfiguration>();
            using (var con = new DataConnection())
            {
                promotionalBanners.AddRange(con.Get<IPromotionalBannerConfiguration>());
            }

            return promotionalBanners;
        }

    }
}
