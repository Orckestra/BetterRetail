using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    public class BaseSearchViewModel : BaseViewModel
    {
        public ProductSearchResultsViewModel ProductSearchResults { get; set; }

        public FacetSettingsViewModel FacetSettings { get; set; }

        public string ListName { get; set; }

        public int MaxItemsPerPage => SearchConfiguration.MaxItemsPerPage;
    }
}
