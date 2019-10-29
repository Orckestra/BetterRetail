using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Search.ViewModels
{
    public class BrandSuggestionsViewModel : BaseViewModel
    {
        public List<BrandSuggestionViewModel> Suggestions { get; set; }
    }
}