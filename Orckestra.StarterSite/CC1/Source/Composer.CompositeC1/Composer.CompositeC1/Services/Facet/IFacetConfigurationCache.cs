using System;
using System.Collections.Generic;
using Orckestra.Composer.Search;

namespace Orckestra.Composer.CompositeC1.Services.Facet
{
    public interface IFacetConfigurationCache
    {
        List<FacetSetting> GetOrAdd(Guid pageId, Func<Guid, List<FacetSetting>> loadMethod);
    };
}