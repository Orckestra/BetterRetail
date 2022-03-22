using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.ContentSearch.ViewModels
{
    public class FacetViewModel: BaseViewModel
    {
        public string Label { get; set; }
        public string Key { get; set;  }
        public List<FacetHitViewModel> Hits { get; set; }
    }
}
