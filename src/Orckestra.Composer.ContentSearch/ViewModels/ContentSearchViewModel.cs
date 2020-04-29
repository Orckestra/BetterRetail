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
    }
}
