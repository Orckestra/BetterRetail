using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Search.ViewModels
{
    public class CategorySuggestionsViewModel : BaseViewModel
    {
        public List<CategorySuggestionViewModel> Suggestions { get; set; }
    }
}