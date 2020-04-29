using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orckestra.Composer.CompositeC1.DataTypes.Facets;
using Orckestra.Composer.CompositeC1.Mappers;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.CompositeC1.Services.DataQuery;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Context;

namespace Orckestra.Composer.CompositeC1.Services.Facet
{
    public class FacetConfigurationContext : IFacetConfigurationContext
    {
        protected ICacheStore<Guid, List<FacetSetting>> Cache{ get; }
        protected HttpContextBase HttpContext { get; }
        protected IDataQueryService DataQueryService { get; }
        private List<FacetSetting> _facetSettings;

        public FacetConfigurationContext(HttpContextBase httpContext, IDataQueryService dataQueryService, ICacheService cacheService)
        {
            HttpContext = httpContext;
            DataQueryService = dataQueryService;
            Cache = cacheService.GetStoreWithDependencies<Guid, List<FacetSetting>>("Facet Configuration", 
                new CacheDependentEntry<IFacet>(), 
                new CacheDependentEntry<IFacetConfiguration>(), 
                new CacheDependentEntry<IFacetConfigurationMeta>(), 
                new CacheDependentEntry<IPromotedFacetValueSetting>()
            );
        }

        public virtual List<FacetSetting> GetFacetSettings()
        {
            if (_facetSettings == null)
            {
                var pageId = HttpContext.GetCurrentPageId();
                _facetSettings = Cache.GetOrAdd(pageId.GetValueOrDefault(), _ => LoadFacetSettings(pageId));
            }
            
            return _facetSettings;
        }


        protected virtual List<FacetSetting> LoadFacetSettings(Guid? pageId)
        {
            var result = new List<FacetSetting>();

            IFacetConfiguration facetConfiguration = null;

            if (pageId.HasValue) // loading from current page
            {
                facetConfiguration = GetFacetConfigurationFromPage(pageId.Value);
            }

            if (facetConfiguration == null) // if no config on page, using any config from db
            {
                facetConfiguration = GetDefaultFacetConfiguration();

                if (facetConfiguration == null) return result; // no facet configs exist
            }

            var facetsIds = GetIds(facetConfiguration.Facets);

            var facets = DataQueryService.Get<IFacet>().Where(f => facetsIds.Contains(f.Id)).ToList();

            var facetsAndData = facets.Select(f => new
            {
                Facet = f,
                DependsOnIds = GetIds(f.DependsOn),
                PromotedIds = GetIds(f.PromotedValues),
            }).ToList();

            var allDependsOnIds = facetsAndData.SelectMany(f => f.DependsOnIds).Distinct().ToList();
            var allDependsOn = LoadDependsOn(allDependsOnIds, facets);

            var allPromotedIds = facetsAndData.SelectMany(f => f.PromotedIds).Distinct().ToList();
            var allPromoted = LoadPromotedFacetValues(allPromotedIds);

            foreach (var facet in facetsAndData)
            {
                var dependsOn = facet.DependsOnIds
                    .Select(id => allDependsOn.TryGetValue(id, out var dependedFacet) ? dependedFacet : null)
                    .Where(f => f != null)
                    .ToList();

                var promoted = facet.PromotedIds
                    .Select(id => allPromoted.TryGetValue(id, out var promotedFacet) ? promotedFacet : null)
                    .Where(f => f != null)
                    .ToList();

                result.Add(FacetsMapper.ConvertToFacetSetting(facet.Facet, dependsOn, promoted));
            }

            return result;
        }

        protected IFacetConfiguration GetFacetConfigurationFromPage(Guid pageId)
        {
            var metaQuery = DataQueryService.Get<IFacetConfigurationMeta>();
            var configQuery = DataQueryService.Get<IFacetConfiguration>();

            var join = from m in metaQuery
                       join c in configQuery on m.Configuration equals c.Id
                       where m.PageId == pageId
                       select c;

            return join.FirstOrDefault();
        }

        protected virtual IFacetConfiguration GetDefaultFacetConfiguration()
        {
            return DataQueryService.Get<IFacetConfiguration>()
                .Where(c => c.IsDefault)
                .FirstOrDefault();
        }

        private Dictionary<Guid, IFacet> LoadDependsOn(List<Guid> dependsOnIds, List<IFacet> existingFacets)
        {
            var result = new Dictionary<Guid, IFacet>();
            for (int i = dependsOnIds.Count - 1; i >= 0; i--)
            {
                var facetId = dependsOnIds[i];
                var facet = existingFacets.Find(f => f.Id == facetId);
                if (facet != null)
                    result[facetId] = facet;
                dependsOnIds.RemoveAt(i);
            }

            if (dependsOnIds.Count > 0)
            {
                var dependsOn = DataQueryService.Get<IFacet>().Where(f => dependsOnIds.Contains(f.Id)).ToList();
                foreach (var facet in dependsOn)
                {
                    result[facet.Id] = facet;
                }
            }

            return result;
        }

        private Dictionary<Guid, IPromotedFacetValueSetting> LoadPromotedFacetValues(List<Guid> allPromotedIds)
        {
            if (allPromotedIds.Count == 0) return new Dictionary<Guid, IPromotedFacetValueSetting>();

            return DataQueryService.Get<IPromotedFacetValueSetting>().Where(f => allPromotedIds.Contains(f.Id)).ToDictionary(f => f.Id, f => f);
        }

        protected static List<Guid> GetIds(string idString)
        {
            return idString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(Guid.Parse)
                .ToList();
        }
    };
}