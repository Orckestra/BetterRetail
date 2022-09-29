using Composite.Data;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.CompositeC1.Services.DataQuery;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Context
{
    public class ProductTileConfigurationContext : IProductTileConfigurationContext
    {
        protected ICacheStore<string, List<IPromotionalRibbonConfiguration>> PromotionalRibbonCache { get; }
        protected ICacheStore<string, List<IPromotionalBannerConfiguration>> PromotionalBannerCache { get; }
        protected ICacheStore<string, List<IVariantColorConfiguration>> VariantColorCache { get; }
        protected HttpContextBase HttpContext { get; }
        protected IDataQueryService DataQueryService { get; }
        public string PromotionalRibbonDefaultBackgroundColor => "bg-dark";
        public string PromotionalRibbonDefaultTextColor => "text-white";
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

            VariantColorCache = cacheService.GetStoreWithDependencies<string, List<IVariantColorConfiguration>>("Variant Colors",
                new CacheDependentEntry<IVariantColorConfiguration>());
        }
        public virtual List<IPromotionalRibbonConfiguration> GetPromotionalRibbonConfigurations()
        {
            return PromotionalRibbonCache.GetOrAdd("PromotionalRibbonSettings", _ => LoadPromotionalRibbonSettings());
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
            return PromotionalBannerCache.GetOrAdd("PromotionalBannerSettings", _ => LoadPromotionalBannerSettings());
        }

        public List<IVariantColorConfiguration> GetVariantColorConfigurations()
        {
            return VariantColorCache.GetOrAdd("VariantColorSettings", _ => LoadVariantColorsSettings());
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

        private List<IVariantColorConfiguration> LoadVariantColorsSettings()
        {
            using (var con = new DataConnection())
            {
                return con.Get<IVariantColorConfiguration>().ToList();
            }
        }

    }
}
