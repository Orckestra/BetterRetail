using Orckestra.Composer.ContentSearch.DataTypes;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.ContentSearch.ViewModels
{
    public class ContentSearchViewModel
    {
        public ContentSearchViewModel()
        {
            Tabs = new List<ContentSearchTabViewModel>();
        }

        public List<ContentSearchTabViewModel> Tabs { get; set; }
        public List<ContentSearchTabViewModel> SuggestedTabs { get; set; }

        public ContentSearchTabViewModel ActiveTab => Tabs.Find(t => t.IsActive);

        public List<FacetViewModel> SelectedFacets { get; set;}

        public List<FacetViewModel> Facets { get; set; }
        public long Total =>  Tabs.Select(t => t.Total).Sum();

        /// <summary>
        /// Gets or sets the selected sort by for the current search results.
        /// </summary>
        /// <value>
        /// The selected sort by.
        /// </value>
        public ISortOption SelectedSortBy { get; set; }

        /// <summary>
        /// Gets or sets the available "sort by" list available to search.
        /// </summary>
        /// <value>
        /// The available sort bys.
        /// </value>
        public IList<ISortOption> AvailableSortBys { get; set; }
    }
}
