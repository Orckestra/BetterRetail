using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Search.ViewModels
{
	public class SearchTermsSuggestionsViewModel : BaseViewModel
    {
		public List<SearchTermsSuggestionViewModel> Suggestions { get; set; }
	}
}
