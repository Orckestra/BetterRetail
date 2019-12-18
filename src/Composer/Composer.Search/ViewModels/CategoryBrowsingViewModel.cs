using System.Collections.Generic;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    public sealed class CategoryBrowsingViewModel : BaseViewModel
    {
        public CategoryBrowsingViewModel()
        {
            ChildCategories = new List<ChildCategoryViewModel>();
        }

        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public SelectedFacets SelectedFacets { get; set; }

        public ProductSearchResultsViewModel ProductSearchResults { get; set; }

        public List<ChildCategoryViewModel> ChildCategories { get; set; }

        public List<string> LandingPageUrls { get; set; }
    }
}
