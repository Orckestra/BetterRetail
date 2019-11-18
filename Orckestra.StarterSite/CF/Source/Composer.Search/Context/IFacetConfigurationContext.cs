using System.Collections.Generic;

namespace Orckestra.Composer.Search.Context
{
    public interface IFacetConfigurationContext
    {
        List<FacetSetting> GetFacetSettings();
    };
}