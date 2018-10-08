using System.Collections.Generic;
using Orckestra.Overture.ServiceModel.Search;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Parameters
{
    public class RemoveSelectedFacetsFromFacetsParam
    {
        public List<Facet> Facets { get; set; }
        public List<SearchFilter> SelectedFacets { get; set; }
        public IList<FacetSetting> FacetSettings { get; set; }
    }
}