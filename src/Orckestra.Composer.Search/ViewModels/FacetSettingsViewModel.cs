using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    public class FacetSettingsViewModel : BaseViewModel
    {
        public SelectedFacets SelectedFacets { get; set; }

        /// <summary>
        /// Category facet values in tree structure
        /// </summary>
        public CategoryFacetValuesTree CategoryFacetValuesTree { get; set; }
    }
}
