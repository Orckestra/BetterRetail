using System.Collections.Generic;

namespace Orckestra.Composer.Search.ViewModels
{
	public class CategorySuggestionViewModel
	{
		public string DisplayName { get; set; }

		public List<string> Parents { get; set; }

		public int Quantity { get; set; }
	}
}