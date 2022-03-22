using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.ContentSearch.ViewModels
{
    public class SearchResultsEntryViewModel: BaseViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string DetailsUrl { get; set; }
        public IDictionary<string, object> FieldsBag { get; set; }
    }
}
