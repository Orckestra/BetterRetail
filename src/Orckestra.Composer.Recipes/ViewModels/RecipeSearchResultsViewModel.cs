using Orckestra.Composer.ContentSearch.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Recipes.ViewModels
{
    public class RecipeSearchResultsViewModel
    {
        public IList<SearchResultsEntryViewModel> SearchResults { get; set; } = new List<SearchResultsEntryViewModel>();
        public long Total { get; set; } = 0;
        public int PagesCount { get; set; } = 0;
    }
}
