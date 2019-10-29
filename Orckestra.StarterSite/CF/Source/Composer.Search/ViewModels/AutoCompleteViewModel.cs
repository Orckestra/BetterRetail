using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Search.ViewModels
{
    public class AutoCompleteViewModel : BaseViewModel
    {
        public List<ProductSearchViewModel> Suggestions { get; set; }
    }
}