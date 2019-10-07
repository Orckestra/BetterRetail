using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Composite.Data;
using Composite.Data.Caching;
using Orckestra.Composer.CompositeC1.DataTypes.Facets;
using Orckestra.Composer.Search;

namespace Orckestra.Composer.CompositeC1.Services.Facet
{
    internal class FacetConfigurationCache : IFacetConfigurationCache, IDisposable
    {
        private readonly Cache<Guid, List<FacetSetting>> _cache = new Cache<Guid, List<FacetSetting>>("Facet Configuration");
        private readonly ConcurrentDictionary<Guid, object> _locks = new ConcurrentDictionary<Guid, object>();

        public FacetConfigurationCache()
        {
            DataEvents<IFacet>.OnAfterAdd += ResetCache;
            DataEvents<IFacet>.OnAfterUpdate += ResetCache;
            DataEvents<IFacet>.OnDeleted += ResetCache;
            DataEvents<IFacetConfiguration>.OnAfterAdd += ResetCache;
            DataEvents<IFacetConfiguration>.OnAfterUpdate += ResetCache;
            DataEvents<IFacetConfiguration>.OnDeleted += ResetCache;
            DataEvents<IFacetConfigurationMeta>.OnAfterAdd += ResetCache;
            DataEvents<IFacetConfigurationMeta>.OnAfterUpdate += ResetCache;
            DataEvents<IFacetConfigurationMeta>.OnDeleted += ResetCache;
            DataEvents<IPromotedFacetValueSetting>.OnAfterAdd += ResetCache;
            DataEvents<IPromotedFacetValueSetting>.OnAfterUpdate += ResetCache;
            DataEvents<IPromotedFacetValueSetting>.OnDeleted += ResetCache;
        }

        public List<FacetSetting> GetOrAdd(Guid pageId, Func<Guid, List<FacetSetting>> loadMethod)
        {
            var key = pageId;
            var value = _cache.Get(key);
            if (value != null)
                return value;

            var syncObj = _locks.GetOrAdd(key, _ => new object());
            lock (syncObj)
            {
                try
                {
                    value = _cache.Get(key);
                    if (value != null)
                        return value;

                    value = loadMethod(key);
                    _cache.Add(key, value);
                    return value;
                }
                finally
                {
                    _locks.TryRemove(key, out _);
                }
            }
        }

        private void ResetCache(object sender, DataEventArgs dataEventArgs)
        {
            _cache.Clear();
        }

        public void Dispose()
        {
            DataEvents<IFacet>.OnAfterAdd -= ResetCache;
            DataEvents<IFacet>.OnAfterUpdate -= ResetCache;
            DataEvents<IFacet>.OnDeleted -= ResetCache;
            DataEvents<IFacetConfiguration>.OnAfterAdd -= ResetCache;
            DataEvents<IFacetConfiguration>.OnAfterUpdate -= ResetCache;
            DataEvents<IFacetConfiguration>.OnDeleted -= ResetCache;
            DataEvents<IFacetConfigurationMeta>.OnAfterAdd -= ResetCache;
            DataEvents<IFacetConfigurationMeta>.OnAfterUpdate -= ResetCache;
            DataEvents<IFacetConfigurationMeta>.OnDeleted -= ResetCache;
            DataEvents<IPromotedFacetValueSetting>.OnAfterAdd -= ResetCache;
            DataEvents<IPromotedFacetValueSetting>.OnAfterUpdate -= ResetCache;
            DataEvents<IPromotedFacetValueSetting>.OnDeleted -= ResetCache;
        }
    };
}