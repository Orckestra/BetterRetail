using Composite.Data;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.CompositeC1.Services.DataQuery;
using Orckestra.Composer.Grocery.DataTypes;
using System.Collections.Generic;
using System.Web;

namespace Orckestra.Composer.Grocery.Context
{
    public class ProductTileConfigurationContext: IProductTileConfigurationContext
    {
        protected ICacheStore<string, List<IPromotionalRibbonConfiguration>> Cache { get; }
        protected HttpContextBase HttpContext { get; }
        protected IDataQueryService DataQueryService { get; }
        private List<IPromotionalRibbonConfiguration> _promotionalRibbonConfigurations;
        public string PromotionalRibbonDefaultBackgroundColor => "bg-dark";
        public string PromotionalRibbonDefaultTextColor => "text-white";

        public ProductTileConfigurationContext(HttpContextBase httpContext, IDataQueryService dataQueryService, ICacheService cacheService)
        {
            HttpContext = httpContext;
            DataQueryService = dataQueryService;
            Cache = cacheService.GetStoreWithDependencies<string, List<IPromotionalRibbonConfiguration>>("Promotional Ribbons",
                new CacheDependentEntry<IPromotionalRibbonConfiguration>());
        }
        public List<IPromotionalRibbonConfiguration> GetPromotionalRibbonConfigurations()
        {
            if (_promotionalRibbonConfigurations == null)
            {
                _promotionalRibbonConfigurations = Cache.GetOrAdd("PromotionalRibbonSettings", _ => LoadPromotionalRibbonSettings());
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


    }
}
