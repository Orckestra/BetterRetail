using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.DataTypes.Facets;
using Orckestra.Composer.CompositeC1.Mappers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Context;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class FacetConfigurationContext : IFacetConfigurationContext
    {
        private readonly HttpContextBase _httpContext;
        private List<FacetSetting> _facetSettings;

        public FacetConfigurationContext(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        private static bool firstTime = true;
        public List<FacetSetting> GetFacetSettings()
        {
            if (firstTime)
            {
                Debugger.Launch();
                firstTime = false;
            }

            return _facetSettings ?? (_facetSettings = LoadFacetSettings());
        }

        private List<FacetSetting> LoadFacetSettings()
        {
            var pageId = GetPageId();

            var result = new List<FacetSetting>();
            using (var conn = new DataConnection())
            {
                var facetConfiguration = GetFacetConfigurationFromPage(pageId, conn);

                if (facetConfiguration == null) // if no config on page, using any config from db
                {
                    facetConfiguration = GetDefaultFacetConfiguration(conn);

                    if (facetConfiguration == null) // no facet configs exist
                        return result;
                }
                    
                var facetsIds = GetIds(facetConfiguration.Facets);

                var facets = conn.Get<IFacet>().Where(f => facetsIds.Contains(f.Id)).ToList();

                var facetsAndData = facets.Select(f => new
                {
                    Facet = f,
                    DependsOnIds = GetIds(f.DependsOn),
                    PromotedIds = GetIds(f.PromotedValues),
                }).ToList();

                var allDependsOnIds = facetsAndData.SelectMany(f => f.DependsOnIds).Distinct().ToList();
                var allDependsOn = LoadDependsOn(allDependsOnIds, facets, conn);

                var allPromotedIds = facetsAndData.SelectMany(f => f.PromotedIds).Distinct().ToList();
                var allPromoted = LoadPromotedFacetValues(allPromotedIds, conn);

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
            }

            return result;
        }

        private IFacetConfiguration GetFacetConfigurationFromPage(Guid pageId, DataConnection conn)
        {
            if (pageId == Guid.Empty)
                return null;

            var metaQuery = conn.Get<IFacetConfigurationMeta>();
            var configQuery = conn.Get<IFacetConfiguration>();

            var join = from m in metaQuery
                       join c in configQuery on m.Configuration equals c.Id
                       where m.PageId == pageId
                       select c;

            return join.FirstOrDefault();
        }

        private IFacetConfiguration GetDefaultFacetConfiguration(DataConnection conn)
        {
            return conn.Get<IFacetConfiguration>()
                .OrderByDescending(c => c.IsDefault)
                .FirstOrDefault();
        }

        private Guid GetPageId()
        {
            const string pageIdKey = "PageRenderer.IPage";
            if (!_httpContext.Items.Contains(pageIdKey))
                return Guid.Empty;

            var page = _httpContext.Items[pageIdKey] as IPage;
            if (page == null)
                return Guid.Empty;

            return page.Id;
        }

        private Dictionary<Guid, IFacet> LoadDependsOn(List<Guid> dependsOnIds, List<IFacet> existingFacets, DataConnection conn)
        {
            var result = new Dictionary<Guid, IFacet>();
            for (int i = dependsOnIds.Count - 1; i >= 0; i--)
            {
                var facetId = dependsOnIds[i];
                var facet = existingFacets.FirstOrDefault(f => f.Id == facetId);
                if (facet != null)
                    result[facetId] = facet;
                dependsOnIds.RemoveAt(i);
            }
                
            if (dependsOnIds.Count > 0)
            {
                var dependsOn = conn.Get<IFacet>().Where(f => dependsOnIds.Contains(f.Id)).ToList();
                foreach (var facet in dependsOn)
                {
                    result[facet.Id] = facet;
                }
            }

            return result;
        }

        private Dictionary<Guid, IPromotedFacetValueSetting> LoadPromotedFacetValues(List<Guid> allPromotedIds, DataConnection conn)
        {
            if (allPromotedIds.Count == 0)
                return new Dictionary<Guid, IPromotedFacetValueSetting>();

            return conn.Get<IPromotedFacetValueSetting>().Where(f => allPromotedIds.Contains(f.Id)).ToDictionary(f => f.Id, f => f);
        }

        private static List<Guid> GetIds(string idString)
        {
            return idString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(Guid.Parse)
                .ToList();
        }

    };
}
